using Microsoft.AspNetCore.Mvc;
using WebAPI.Attributes;

namespace WebAPI.Controllers
{
    [LoginRequired]
    public class SwaggerController : Controller
    {
        [IgnoreApi]
        public IActionResult ReDoc()
        {
            return View();
        }
    }
}
