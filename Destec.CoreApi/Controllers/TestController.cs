using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Destec.CoreApi.Controllers
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        [HttpGet]
        public IActionResult GetList()
        {
            using (var algorithm = SHA256.Create())
            {
                int count = 0;
                count = Enumerable.Range(0, 20000).Select(c => SHA256.Create().ComputeHash(new Guid().ToByteArray())).Count();
                return Ok(count);
            }
        }

        [HttpGet]
        [Route("echo/{id:int}")]
        public IActionResult GetEcho(int id)
        {
            return Ok(id);
        }
    }
}
