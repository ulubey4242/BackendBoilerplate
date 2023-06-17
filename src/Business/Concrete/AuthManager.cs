using Business.Abstract;
using Business.Constants;
using Business.Utilities.Security.Jwt;
using Core.Extensions;
using Core.Utilities.Results;
using Entities.Concrete;
using Entities.Dtos;
using System;

namespace Business.Concrete
{
    public class AuthManager : IAuthService
    {
        private IUserService _userService;
        private ITokenHelper _tokenHelper;

        public AuthManager(IUserService userService, ITokenHelper tokenHelper)
        {
            _userService = userService;
            _tokenHelper = tokenHelper;
        }

        public IDataResult<int> Register(UserForRegisterDto userForRegisterDto)
        {
            if (_userService.GetByMail(userForRegisterDto.Email) != null)
                return new ErrorDataResult<int>(0, Messages.UserAlreadyExists);

            var user = new User
            {
                Email = userForRegisterDto.Email,
                Password = CryptExtensions.Crypt(userForRegisterDto.Password),
                FirstName = userForRegisterDto.FirstName,
                LastName = userForRegisterDto.LastName
            };

            _userService.Add(user);

            return new SuccessDataResult<int>(user.Id, Messages.UserRegistered);
        }

        public IDataResult<UserForLoginDataDto> Login(UserForLoginDto userForLoginDto)
        {
            var userToCheck = _userService.GetByMail(userForLoginDto.Email);

            if (userToCheck == null)
                return new ErrorDataResult<UserForLoginDataDto>(Messages.UserNotFound);

            var userToPasswordCheck = userToCheck.Password == CryptExtensions.Crypt(userForLoginDto.Password) ? true : false;

            if (userToPasswordCheck == false)
                return new ErrorDataResult<UserForLoginDataDto>(Messages.PasswordError);

            var result = CreateAccessToken(userToCheck);
            if (result.Success)
            {
                ApplyToken(userToCheck, result.Data);
                return new SuccessDataResult<UserForLoginDataDto>(new UserForLoginDataDto(result.Data, userToCheck.Id, userToCheck.Email, userToCheck.FirstName, userToCheck.LastName, userToCheck.UserOperationClaims), Messages.SuccessfulLogin);
            }

            return new ErrorDataResult<UserForLoginDataDto>(result.Message);
        }

        public IResult UserExist(string email)
        {
            if (_userService.GetByMail(email) == null)
                return new ErrorResult(Messages.UserNotFound);

            return new SuccessResult();
        }

        public IDataResult<string> CreateAccessToken(User user)
        {
            var claims = _userService.GetClaims(user);
            var accessToken = user.Token;

            if (string.IsNullOrEmpty(user.Token) || (user.TokenExpiredAt.HasValue && user.TokenExpiredAt.Value < System.DateTime.UtcNow))
                accessToken = _tokenHelper.CreateToken(user, claims);

            return new SuccessDataResult<string>(accessToken, Messages.AccessTokenCreated);
        }

        public IResult ApplyToken(User user, string token)
        {
            _userService.UpdateToken(user, token);

            return new SuccessResult();
        }

        public IResult TokenIsExpired(AuthForTokenExpiredDto authForTokenExpiredDto)
        {
            var user = _userService.GetByToken(authForTokenExpiredDto);

            if (user == null)
                return new SuccessResult(Messages.UserNotFound);

            if (string.IsNullOrEmpty(user.Token) || (user.TokenExpiredAt.HasValue && user.TokenExpiredAt.Value < System.DateTime.UtcNow))
                return new SuccessResult();

            return new ErrorResult();
        }
    }
}
