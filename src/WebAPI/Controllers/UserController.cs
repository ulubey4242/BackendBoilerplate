using Business.Abstract;
using Core.Utilities.Results;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using WebAPI.Attributes;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [JWTAuth]
    public class UserController : Controller
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("get")]
        [SwaggerOperation(Summary = "Get")]
        public IActionResult Get()
        {
            var result = _userService.Get();
            return Ok(result);
        }

        [HttpGet("getbyid")]
        [SwaggerOperation(Summary = "GetById")]
        public IActionResult GetById(UserForIdDto dto)
        {
            var result = _userService.GetById(dto);
            return Ok(result);
        }

        [HttpPost("Update")]
        [SwaggerOperation(Summary = "Update")]
        public IActionResult Update(User user)
        {
            var result = _userService.Update(user);
            return Ok(result);
        }
    }
}
