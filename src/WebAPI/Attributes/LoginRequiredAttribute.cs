using Business.Utilities.Security.Jwt;
using Core.DataAccess.Abstract;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace WebAPI.Attributes
{
    public class LoginRequiredAttribute : TypeFilterAttribute
    {
        #region Ctor

        public LoginRequiredAttribute() : base(typeof(LoginRequiredFilter))
        {
        }

        #endregion

        #region Nested filter

        private class LoginRequiredFilter : IAuthorizationFilter
        {
            #region Fields

            private readonly ITokenHelper _tokenHelper;
            private readonly IUserDal _userDal;

            #endregion

            #region Ctor

            public LoginRequiredFilter(ITokenHelper tokenHelper,
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

                var loginURL = new PathString("/Login");

                try
                {
                    if (filterContext.Filters.Any(filter => filter is LoginRequiredFilter))
                    {
                        if (filterContext.HttpContext.Request.Cookies.TryGetValue("token", out string bearer))
                        {
                            var token = bearer.Replace("Bearer ", "");

                            if (_tokenHelper.Validate(token))
                            {
                                var id = _tokenHelper.GetClaim(token, ClaimNames.Id);
                                var email = _tokenHelper.GetClaim(token, ClaimNames.Email);

                                var user = _userDal.Get(x => !x.Deleted && x.RowGuid.ToString() == id && x.Email == email &&
                                                            x.Token == token && x.TokenExpiredAt >= DateTime.UtcNow);

                                if (user == null)
                                    filterContext.Result = new RedirectResult(loginURL);
                            }
                            else
                                filterContext.Result = new RedirectResult(loginURL);
                        }
                        else
                            filterContext.Result = new RedirectResult(loginURL);
                    }
                }
                catch
                {
                    filterContext.Result = new RedirectResult(loginURL);
                }
            }

            #endregion
        }

        #endregion
    }
}
