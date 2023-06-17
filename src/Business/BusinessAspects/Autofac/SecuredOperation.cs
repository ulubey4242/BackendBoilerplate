using Business.Constants;
using Business.Utilities.Security.Jwt;
using Castle.DynamicProxy;
using Core.DataAccess.Abstract;
using Core.Utilities.Interceptors;
using Core.Utilities.IoC;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Business.BusinessAspects.Autofac
{
    public class SecuredOperation : MethodInterception
    {
        private string[] _roles;
        private IHttpContextAccessor _httpContextAccessor;
        private ITokenHelper _tokenHelper;
        private IUserDal _userDal;

        public SecuredOperation(string roles)
        {
            _roles = roles.Split(',');

            _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();
            _tokenHelper = ServiceTool.ServiceProvider.GetService<ITokenHelper>();

            _userDal = ServiceTool.ServiceProvider.GetService<IUserDal>();
        }

        protected override void OnBefore(IInvocation invocation)
        {
            try
            {
                var authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault() ?? "";

                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    var token = authHeader.Replace("Bearer ", "");

                    if (_tokenHelper.Validate(token))
                    {
                        var userId = _tokenHelper.GetClaim(token, ClaimNames.Id);
                        var userGuid = new Guid(userId);

                        var roleExist = _userDal.GetList(user =>
                                                user.RowGuid == userGuid &&
                                                user.UserOperationClaims.Any(uoc => _roles.Contains(uoc.OperationClaim.Name ?? "")))
                                            .Include(x => x.UserOperationClaims)
                                            .ThenInclude(x => x.OperationClaim)
                                            .Count() > 0;

                        if (roleExist) return;
                    }
                }
            }
            catch { }

            throw new Exception(Messages.AuthorizationDenied);
        }
    }
}
