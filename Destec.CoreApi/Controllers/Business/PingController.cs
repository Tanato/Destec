using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Destec.CoreApi.Controllers.Business
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class PingController : Controller
    {
        [HttpGet]
        public int Get()
        {
            return 1;
        }
    }
}
