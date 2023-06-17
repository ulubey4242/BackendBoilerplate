using Core.Utilities.Security;
using Core.Utilities.Security.Encyption;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Core.Extensions
{
    public static class CryptExtensions
    {
        public static string Crypt(this int input)
        {
            return CryptPassword(input.ToString());
        }

        public static string Crypt(this string input)
        {
            return CryptPassword(input);
        }

        public static string Decrypt(this string input)
        {
            try
            {
                return DecryptPassword(input);
            }
            catch { return ""; }
        }

        private static string CryptPassword(string input)
        {
            const string encryptionKey = "MAKVKKKBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(input);
            using Aes encryptor = Aes.Create();
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey,
                new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using MemoryStream ms = new MemoryStream();
            using (CryptoStream cs  = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(clearBytes, 0, clearBytes.Length);
                cs.Close();
            }

            input = Convert.ToBase64String((ms.ToArray()));

            return input;
        }

        private static string DecryptPassword(string input)
        {
            if ((input ?? "") == "")
                return "";

            const string encryptionKey = "MAKVKKKBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(input);
            using Aes encryptor = Aes.Create();
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey,
                new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using MemoryStream ms = new MemoryStream();
            using (CryptoStream cs  = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(cipherBytes, 0, cipherBytes.Length);
                cs.Close();
            }
            input = Encoding.Unicode.GetString((ms.ToArray()));

            return input;
        }
    }
}
