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
    public class OperationClaimController : Controller
    {
        private IOperationClaimService _operationClaim;

        public OperationClaimController(IOperationClaimService operationClaim)
        {
            _operationClaim = operationClaim;
        }

        [HttpGet("get")]
        [SwaggerOperation(Summary = "Get")]
        public IActionResult Get()
        {
            var result = _operationClaim.Get();
            return Ok(result);
        }
    }
}
