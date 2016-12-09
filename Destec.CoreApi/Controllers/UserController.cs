using System.Collections.Generic;
using System.Linq;
using Destec.CoreApi.Models.Business;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Destec.CoreApi.Models;
using System;

namespace Destec.CoreApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private UserManager<User> userManager;
        private readonly ApplicationDbContext db;

        public UserController(ApplicationDbContext db, UserManager<User> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var roles = db.Roles.ToList();

            var users = db.Users.Include(x => x.Roles).OrderBy(x => x.UserName).ToList()
                .Select(user => new
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Name = user.Name,
                    BirthDate = user.BirthDate,
                    Email = user.Email,
                    Login = user.UserName,
                    Roles = user.Roles.Select(x => x.RoleId),
                    Inativo = user.Inativo,
                    Perfis = string.Join(", ", roles.Where(x => user.Roles.Any(z => z.RoleId == x.Id))),
                });


            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var user = db.Users
                            .Include(x => x.Roles)
                            .SingleOrDefault(x => x.Id.Equals(id));

            if (user != null)
            {
                var rolesNames = string.Join(", ", db.Roles.Where(x => x.Users.Any(z => z.UserId == user.Id)));

                var result = new
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Name = user.Name,
                    BirthDate = user.BirthDate,
                    Email = user.Email,
                    Login = user.UserName,
                    Roles = user.Roles.Select(x => x.RoleId),
                    Inativo = user.Inativo,
                    Perfis = rolesNames,
                };
                return Ok(result);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("{userId}")]
        public void AddProfileToUser(string userId, [FromBody]IEnumerable<string> roles)
        {
            var user = db.Users.SingleOrDefault(x => x.Id.Equals(userId));
            userManager.AddToRolesAsync(user, roles);
            db.SaveChanges();
        }

        [HttpGet("select")]
        public IActionResult GetKeyValue([FromQuery] string filter)
        {
            var result = db.Users.Where(x => string.IsNullOrEmpty(filter)
                                             || x.Name.ContainsIgnoreNonSpacing(filter))
                                    .OrderBy(x => x.Name)
                                    .Select(x => new KeyValuePair<string, string>(x.Id, x.Name)).ToList();

            return Ok(result);
        }

        [HttpGet("select/{id}")]
        public IActionResult GetKeyValueId(string id)
        {
            var result = db.Users.Select(x => new KeyValuePair<string, string>(x.Id, x.Name)).Single(x => x.Key == id);
            return Ok(result);
        }
    }
}
