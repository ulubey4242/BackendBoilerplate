using Business.Abstract;
using Core.Utilities.Results;
using Entities.Dtos;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController:Controller
    {
        private IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [SwaggerOperation(Summary = "Login")]
        public IActionResult Login(UserForLoginDto userForLoginDto)
        {
            var result = _authService.Login(userForLoginDto);
            return Ok(result);
        }

        [HttpPost("register")]
        [SwaggerOperation(Summary = "Register")]
        public IActionResult Register(UserForRegisterDto userForRegisterDto)
        {
            var result = _authService.Register(userForRegisterDto);
            return Ok(result);
        }

        [HttpPost("token_is_expired")]
        [SwaggerOperation(Summary = "TokenIsExpired")]
        public IActionResult TokenIsExpired(AuthForTokenExpiredDto authForTokenExpiredDto)
        {
            var result = _authService.TokenIsExpired(authForTokenExpiredDto);
            return Ok(result);
        }
    }
}
