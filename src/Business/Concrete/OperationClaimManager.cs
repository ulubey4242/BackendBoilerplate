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

namespace Business.Concrete
{
    public class OperationClaimManager : IOperationClaimService
    {
        public OperationClaimManager() { }

        [SecuredOperation("Admin")]
        [LogAspect(typeof(DatabaseLogger))]
        public IDataResult<List<OperationClaim>> Get()
        {
            var _operationClaimDal = ServiceTool.ServiceProvider.GetService<IOperationClaimDal>();
            var data = _operationClaimDal.GetList().ToList();
            return new SuccessDataResult<List<OperationClaim>>(data);
        }
    }
}
