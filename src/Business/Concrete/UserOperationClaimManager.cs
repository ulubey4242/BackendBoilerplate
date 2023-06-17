using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Log4Net.Loggers;
using Core.Utilities.Results;
using Core.Utilities.IoC;
using DataAccess.Abstract;
using Entities.Concrete;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Business.Constants;
using Core.Aspects.Autofac.Validation;

namespace Business.Concrete
{
    public class UserOperationClaimManager : IUserOperationClaimService
    {
        public UserOperationClaimManager() { }

        [SecuredOperation("Admin")]
        [LogAspect(typeof(DatabaseLogger))]
        public IDataResult<List<UserOperationClaim>> Get()
        {
            var _userOperationClaimDal = ServiceTool.ServiceProvider.GetService<IUserOperationClaimDal>();
            var data = _userOperationClaimDal.GetList().ToList();
            return new SuccessDataResult<List<UserOperationClaim>>(data);
        }

        [SecuredOperation("Admin")]
        [LogAspect(typeof(DatabaseLogger))]
        public IResult Add(UserOperationClaim userOperationClaim)
        {
            var _userOperationClaimDal = ServiceTool.ServiceProvider.GetService<IUserOperationClaimDal>();
            _userOperationClaimDal.Add(userOperationClaim);

            return new SuccessResult(Messages.UserRegistered);
        }

        [SecuredOperation("Admin")]
        [LogAspect(typeof(DatabaseLogger))]
        public IResult Delete(UserOperationClaim userOperationClaim)
        {
            var _userOperationClaimDal = ServiceTool.ServiceProvider.GetService<IUserOperationClaimDal>();
            _userOperationClaimDal.Delete(userOperationClaim);

            return new SuccessResult(Messages.UserEdited);
        }
    }
}
