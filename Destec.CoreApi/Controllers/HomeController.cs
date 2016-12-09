using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Destec.CoreApi.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
            return Redirect("/swagger/ui/index.html");
        }
    }
}
