using Business.Abstract;
using Business.Constants;
using Business.Utilities.Predicate;
using Castle.Core.Internal;
using Core.Extensions;
using Core.Utilities.IoC;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Concrete
{
    public class UserManager : IUserService
    {
        public IConfiguration Configuration { get; }
        private TokenOptions _tokenOptions;

        public UserManager(IConfiguration configuration)
        {
            Configuration = configuration;

            _tokenOptions = Configuration.GetSection("TokenOptions").Get<TokenOptions>();
        }

        public IDataResult<List<User>> Get()
        {
            var _userDal = ServiceTool.ServiceProvider.GetService<IUserDal>();
            var data = _userDal.GetList().ToList();
            data.ForEach(x => x.Password = CryptExtensions.Decrypt(x.Password));
            return new SuccessDataResult<List<User>>(data);
        }

        public IDataResult<User> GetById(UserForIdDto dto)
        {
            var _userDal = ServiceTool.ServiceProvider.GetService<IUserDal>();
            var data = _userDal.GetList(x => x.Id == dto.UserId).FirstOrDefault();
            if (data != null)
                data.Password = CryptExtensions.Decrypt(data.Password);

            return new SuccessDataResult<User>(data);
        }

        public IResult Update(User user)
        {
            var _userDal = ServiceTool.ServiceProvider.GetService<IUserDal>();
            user.Password = CryptExtensions.Crypt(user.Password);
            _userDal.Update(user);
            return new SuccessResult(Messages.UserEdited);
        }

        public List<OperationClaim> GetClaims(User user)
        {
            if (user == null)
                return new List<OperationClaim>();

            var _userDal = ServiceTool.ServiceProvider.GetService<IUserDal>();

            return _userDal.GetList(x => x.RowGuid == user.RowGuid)
                                .Include(x => x.UserOperationClaims)
                                .ThenInclude(x => x.OperationClaim)
                                .Select(x => x.UserOperationClaims.Select(y => y.OperationClaim).ToList())
                                .FirstOrDefault();
        }

        public void Add(User user)
        {
            var _userDal = ServiceTool.ServiceProvider.GetService<IUserDal>();

            _userDal.Add(user);
        }

        public User GetByMail(string email)
        {
            var _userDal = ServiceTool.ServiceProvider.GetService<IUserDal>();

            return _userDal.GetList(x => x.Email == email)
                                .Include(x => x.UserOperationClaims)
                                .ThenInclude(x => x.OperationClaim)
                                .FirstOrDefault();
        }

        public User GetByToken(AuthForTokenExpiredDto authForTokenExpiredDto)
        {
            var _userDal = ServiceTool.ServiceProvider.GetService<IUserDal>();

            return _userDal.GetList(x => x.Token == authForTokenExpiredDto.Token)
                                .Include(x => x.UserOperationClaims)
                                .ThenInclude(x => x.OperationClaim)
                                .FirstOrDefault();
        }

        public void UpdateToken(User user, string token)
        {
            if (user == null)
                return;

            var _userDal = ServiceTool.ServiceProvider.GetService<IUserDal>();

            var record = _userDal.Get(x => x.RowGuid == user.RowGuid);

            if (record == null)
                return;

            record.Token = token;
            record.TokenExpiredAt = DateTime.UtcNow.AddMinutes(_tokenOptions.AccessTokenExpiration);

            _userDal.Update(record);
        }
    }
}
