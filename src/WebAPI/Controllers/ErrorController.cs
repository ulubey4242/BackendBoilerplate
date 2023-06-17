using Core.Utilities.Results;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Attributes;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [IgnoreApi]
        [HttpGet("Authorization")]
        public IActionResult Authorization()
        {
            return Ok(new ErrorResult(message: "Authorization Error"));
        }
    }
}
