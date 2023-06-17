using Business.Utilities.Security.Jwt;
using Core.DataAccess.Abstract;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;

namespace WebAPI.Attributes
{
    public class JWTAuthAttribute : TypeFilterAttribute
    {
        #region Ctor

        public JWTAuthAttribute() : base(typeof(AuthenticationFilter))
        {
        }

        #endregion

        #region Nested filter

        private class AuthenticationFilter : IAuthorizationFilter
        {
            #region Fields

            private readonly ITokenHelper _tokenHelper;
            private readonly IUserDal _userDal;

            #endregion

            #region Ctor

            public AuthenticationFilter(ITokenHelper tokenHelper,
                IUserDal userDal)
            {
                _tokenHelper = tokenHelper;
                _userDal = userDal;
            }

            #endregion

            #region Methods

            public void OnAuthorization(AuthorizationFilterContext filterContext)
            {
                if (filterContext == null)
                    throw new ArgumentNullException(nameof(filterContext));

                var errorURL = new PathString("/Error/Authorization");

                try
                {
                    if (filterContext.Filters.Any(filter => filter is AuthenticationFilter))
                    {
                        if (filterContext.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues Authorization))
                        {
                            var bearer = Authorization.ToString();
                            var token = bearer.Replace("Bearer ", "");

                            if (_tokenHelper.Validate(token))
                            {
                                var id = _tokenHelper.GetClaim(token, ClaimNames.Id);
                                var email = _tokenHelper.GetClaim(token, ClaimNames.Email);

                                var user = _userDal.Get(x => !x.Deleted && x.RowGuid.ToString() == id && x.Email == email &&
                                                            x.Token == token && x.TokenExpiredAt >= DateTime.UtcNow);

                                if (user == null)
                                    filterContext.Result = new RedirectResult(errorURL);
                            }
                            else
                                filterContext.Result = new RedirectResult(errorURL);
                        }
                        else
                            filterContext.Result = new RedirectResult(errorURL);
                    }
                }
                catch
                {
                    filterContext.Result = new RedirectResult(errorURL);
                }
            }

            #endregion
        }

        #endregion
    }
}
