using Destec.CoreApi.Models.Business;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Destec.CoreApi.Models
{
    public class User : IdentityUser
    {
        public User()
        {
            Inativo = false;
        }

        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public string Password { get; set; }
        public bool Inativo { get; set; }
        
        public List<Log> UserLogs { get; set; }
    }
}