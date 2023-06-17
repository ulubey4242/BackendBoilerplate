using System;
using System.IO;

namespace Crypto
{
    /// <summary>
    /// Provides a base for symmetric encryption algorithms
    /// </summary>
    public interface ISymmetricEncryptionProvider
    {

        /// <summary>
        /// Symmetric encryption routine
        /// </summary>
        /// <param name="data">The data that should get encrypted</param>
        /// <returns>The encrypted data</returns>
        byte[] Encrypt(byte[] data);


        /// <summary>
        /// Symmetric decryption routine 
        /// </summary>
        /// <param name="data">The data that should get decrypted</param>
        /// <returns>The decrypted data</returns>
        byte[] Decrypt(byte[] data);

    }

    /// <summary>
    /// Like TEA, XTEA is a 64-bit block Feistel cipher with a 128-bit key and a suggested
    /// 64 rounds. Several differences from TEA are apparent, including a somewhat
    /// more complex key-schedule and a rearrangement of the shifts, XORs, and additions.
    /// 
    /// More information can be found here:
    /// + https://en.wikipedia.org/wiki/XTEA
    /// + http://www.tayloredge.com/reference/Mathematics/TEA-XTEA.pdf
    /// </summary>
    public class XTEA : ISymmetricEncryptionProvider
    {
        /// <summary>
        /// The 128 bit key used for encryption and decryption
        /// </summary>
        private readonly uint[] _key;


        /// <summary>
        /// The number of rounds, default is 32 because each iteration performs two Feistel-cipher rounds.
        /// </summary>
        private readonly uint _cycles;


        /// <summary>
        /// XTEA operates with a block size of 8 bytes
        /// </summary>
        private readonly uint _blockSize = 8;


        /// <summary>
        /// The delta is derived from the golden ratio where delta = (sqrt(2) - 1) * 2^31
        /// A different multiple of delta is used in each round so that no bit of
        /// the multiple will not change frequently
        /// </summary>
        private const uint Delta = 0x9E3779B9;


        /// <summary>
        /// Instantiate new XTEA object for encryption/decryption
        /// </summary>
        /// <param name="key">The encryption/decryption key</param>
        /// <param name="cycles">Number of cycles performed, default is 32</param>
        public XTEA(byte[] key, uint cycles = 32)
        {
            _key = GenerateKey(key);
            _cycles = cycles;
        }


        /// <summary>
        /// Calculates the next multiple of the block size of the input data because
        /// XTEA is a 64-bit cipher.
        /// </summary>
        /// <param name="length">Input data size</param>
        /// <returns>Input data extended to the next multiple of the block size.</returns>
        private uint NextMultipleOfBlockSize(uint length)
        {
            return (length + 7) / _blockSize * _blockSize;
        }


        /// <summary>
        /// Encrypts the provided data with XTEA
        /// </summary>
        /// <param name="data">The data to encrypt</param>
        /// <returns>Encrypted data as byte array</returns>
        public byte[] Encrypt(byte[] data)
        {
            var blockBuffer = new uint[2];
            var dataBuffer = new byte[NextMultipleOfBlockSize((uint)data.Length + 4)];
            var lengthBuffer = BitConverter.GetBytes(data.Length);

            Buffer.BlockCopy(lengthBuffer, 0, dataBuffer, 0, lengthBuffer.Length);
            Buffer.BlockCopy(data, 0, dataBuffer, lengthBuffer.Length, data.Length);

            using (var memoryStream = new MemoryStream(dataBuffer))
            using (var binaryWriter = new BinaryWriter(memoryStream))
                for (uint i = 0; i < dataBuffer.Length; i += _blockSize)
                {
                    blockBuffer[0] = BitConverter.ToUInt32(dataBuffer, (int)i);
                    blockBuffer[1] = BitConverter.ToUInt32(dataBuffer, (int)i + 4);

                    Encode(_cycles, blockBuffer, _key);

                    binaryWriter.Write(blockBuffer[0]);
                    binaryWriter.Write(blockBuffer[1]);
                }

            return dataBuffer;
        }


        /// <summary>
        /// Decrypts the provided data with XTEA
        /// </summary>
        /// <param name="data">The data to decrypt</param>
        /// <returns>The decrypted data as byte array</returns>
        public byte[] Decrypt(byte[] data)
        {
            // Encrypted data size must be a multiple of the block size
            if (data.Length % _blockSize != 0)
                throw new ArgumentOutOfRangeException(nameof(data));

            var blockBuffer = new uint[2];
            var buffer = new byte[data.Length];

            Buffer.BlockCopy(data, 0, buffer, 0, data.Length);

            using (var memoryStream = new MemoryStream(buffer))
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                for (uint i = 0; i < buffer.Length; i += _blockSize)
                {
                    blockBuffer[0] = BitConverter.ToUInt32(buffer, (int)i);
                    blockBuffer[1] = BitConverter.ToUInt32(buffer, (int)i + 4);

                    Decode(_cycles, blockBuffer, _key);

                    binaryWriter.Write(blockBuffer[0]);
                    binaryWriter.Write(blockBuffer[1]);
                }
            }

            // Verify if length of output data is valid
            var length = BitConverter.ToUInt32(buffer, 0);
            VerifyDataLength(length, buffer.Length, 4);

            // Trim first 4 bytes of output data            
            return TrimOutputData(length, buffer, 4);
        }


        /// <summary>
        /// Removes the first n bytes from the buffer
        /// </summary>
        /// <param name="length">Length of the output buffer</param>
        /// <param name="buffer">The output buffer</param>
        /// <param name="trimSize">Number of bytes to trim from the start of the buffer</param>
        /// <returns>Trimmed output buffer array</returns>
        private byte[] TrimOutputData(uint length, byte[] buffer, int trimSize)
        {
            var result = new byte[length];
            Buffer.BlockCopy(buffer, trimSize, result, 0, (int)length);
            return result;
        }


        /// <summary>
        /// Compares the length of the output data from a specified offset to the length of the buffer
        /// </summary>
        /// <param name="dataLength">Length of the output data</param>
        /// <param name="bufferLength">Length of the buffer</param>
        /// <param name="offset">The offset</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if buffer data is corrupted</exception>
        private void VerifyDataLength(uint dataLength, int bufferLength, uint offset)
        {
            if (dataLength > bufferLength - offset)
                throw new ArgumentOutOfRangeException(nameof(bufferLength));
        }


        /// <summary>
        /// Transforms the key to uint[]
        /// </summary>
        /// <returns>Transformed key</returns>
        private uint[] GenerateKey(byte[] key)
        {
            if (key.Length != 16)
                throw new ArgumentOutOfRangeException(nameof(key));

            return new[]
            {
                BitConverter.ToUInt32(key, 0), BitConverter.ToUInt32(key, 4),
                BitConverter.ToUInt32(key, 8), BitConverter.ToUInt32(key, 12)
            };
        }


        /// <summary>
        /// TEA inplace encoding routine of the provided data array.
        /// </summary>
        /// <param name="rounds">The number of encryption rounds, default is 32.</param>
        /// <param name="v">The data array containing two values.</param>
        /// <param name="k">The key array containing 4 values.</param>
        private void Encode(uint rounds, uint[] v, uint[] k)
        {
            uint sum = 0;
            uint v0 = v[0], v1 = v[1];
            for (int i = 0; i < rounds; i++)
            {
                v0 += (((v1 << 4) ^ (v1 >> 5)) + v1) ^ (sum + k[sum & 3]);
                sum += Delta;
                v1 += (((v0 << 4) ^ (v0 >> 5)) + v0) ^ (sum + k[(sum >> 11) & 3]);
            }

            v[0] = v0;
            v[1] = v1;
        }


        /// <summary>
        /// TEA inplace decoding routine of the provided data array.
        /// </summary>
        /// <param name="rounds">The number of encryption rounds, default is 32.</param>
        /// <param name="v">The data array containing two values.</param>
        /// <param name="k">The key array containing 4 values.</param>
        private void Decode(uint rounds, uint[] v, uint[] k)
        {
            uint sum = Delta * rounds;
            uint v0 = v[0], v1 = v[1];
            for (int i = 0; i < rounds; i++)
            {
                v1 -= (((v0 << 4) ^ (v0 >> 5)) + v0) ^ (sum + k[(sum >> 11) & 3]);
                sum -= Delta;
                v0 -= (((v1 << 4) ^ (v1 >> 5)) + v1) ^ (sum + k[sum & 3]);
            }

            v[0] = v0;
            v[1] = v1;
        }

    }
}