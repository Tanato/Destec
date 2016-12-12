using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Destec.CoreApi.Controllers.Business
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class PingController : Controller
    {
        public int Get()
        {
            return 1;
        }
    }
}
