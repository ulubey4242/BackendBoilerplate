using Business.Abstract;
using Core.Utilities.Results;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserOperationClaimController : Controller
    {
        private IUserOperationClaimService _userOperationClaim;

        public UserOperationClaimController(IUserOperationClaimService UserOperationClaim)
        {
            _userOperationClaim = UserOperationClaim;
        }

        [HttpGet("get")]
        [SwaggerOperation(Summary = "Get")]
        public IActionResult Get()
        {
            var result = _userOperationClaim.Get();
            return Ok(result);
        }

        [HttpPost("add")]
        [SwaggerOperation(Summary = "Add")]
        public IActionResult Add(UserOperationClaim userOperationClaim)
        {
            var result = _userOperationClaim.Add(userOperationClaim);
            return Ok(result);
        }

        [HttpPost("delete")]
        [SwaggerOperation(Summary = "Delete")]
        public IActionResult Delete(UserOperationClaim userOperationClaim)
        {
            var result = _userOperationClaim.Delete(userOperationClaim);
            return Ok(result);
        }
    }
}
