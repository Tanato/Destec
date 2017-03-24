using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Destec.CoreApi.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
            return Redirect("/index.html");
        }
    }
}
