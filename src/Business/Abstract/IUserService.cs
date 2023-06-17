using Core.Utilities.Results;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Abstract
{
    public interface IUserService
    {
        IDataResult<List<User>> Get();
        IDataResult<User> GetById(UserForIdDto dto);
        List<OperationClaim> GetClaims(User user);
        void Add(User user);
        User GetByMail(string email);
        void UpdateToken(User user, string token);
        User GetByToken(AuthForTokenExpiredDto authForTokenExpiredDto);
        IResult Update(User user);
    }
}
