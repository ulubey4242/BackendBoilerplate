using Core.Utilities.Results;
using Entities.Concrete;
using Entities.Dtos;

namespace Business.Abstract
{
    public interface IAuthService
    {
        IDataResult<int> Register(UserForRegisterDto userForRegisterDto);
        IDataResult<UserForLoginDataDto> Login(UserForLoginDto userForLoginDto);
        IResult UserExist(string email);
        IDataResult<string> CreateAccessToken(User user);
        IResult ApplyToken(User user, string token);
        IResult TokenIsExpired(AuthForTokenExpiredDto authForTokenExpiredDto);
    }
}
