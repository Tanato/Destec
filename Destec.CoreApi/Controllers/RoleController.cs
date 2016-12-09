using System.Collections.Generic;
using System.Linq;
using Destec.CoreApi.Models.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Destec.CoreApi.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Destec.CoreApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<User> userManager;

        public RoleController(ApplicationDbContext db, UserManager<User> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        [HttpGet]
        public ActionResult Get()
        {
            return Ok(db.Roles.Select(x => new { Id = x.Id, Name = x.Name }));
        }

        [HttpGet("{id}")]
        public ActionResult GetRoleByUserId(string id)
        {
            return Ok(db.Roles.Include(x => x.Users).Where(x => x.Id == x.Users.SingleOrDefault(z => z.UserId.Equals(id)).RoleId));
        }

        [Route("user")]
        [HttpGet]
        public async Task<IActionResult> GetRolesByUser()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            var userRoles = db.Users.Include(x => x.Roles).Single(x => x.Id == user.Id).Roles;
            var result = db.Roles.Select(x => new { Id = x.Id, Name = x.Name, Checked = userRoles.Select(z => z.RoleId).Contains(x.Id) });
            return Ok(result);
        }
    }
}