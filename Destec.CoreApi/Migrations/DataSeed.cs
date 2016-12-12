using Destec.CoreApi.Models;
using Destec.CoreApi.Models.Business;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace Destec.CoreApi.Migrations
{
    public static class DataSeed
    {
        public static async void EnsureSeedIdentityAsync(this IApplicationBuilder app)
        {
            var context = app.ApplicationServices.GetService<ApplicationDbContext>();

            if (context != null)
            {
                context.Database.Migrate();
            }

            var roleManager = app.ApplicationServices.GetService<RoleManager<IdentityRole>>();
            var userManager = app.ApplicationServices.GetService<UserManager<User>>();

            var roles = new[] { "Administrativo" };

            foreach (var item in roles)
            {
                if (!context.Roles.Any(x => x.Name == item))
                    await roleManager.CreateAsync(new IdentityRole { Name = item });
            }

            if (!context.Users.Any())
            {
                var userResult = await userManager.CreateAsync(new User { Name = "Tanato Cartaxo", UserName = "Admin" }, "123Admin");
            }
            
            if (!context.Funcionarios.Any())
            {
                context.Funcionarios.Add(new Funcionario
                {
                    Nome = "Tanato Cartaxo",
                    Codigo = "142",
                    Inativo = false,
                    AtividadeAtualId = null,
                    TarefaAssociadas = new TarefaAssociada[]
                    {
                        new TarefaAssociada { KitId = 1, GrupoKit = 1},
                    }.ToList(),
                });
                context.Funcionarios.Add(new Funcionario
                {
                    Nome = "Geraldo Sergio",
                    Codigo = "955",
                    Inativo = false,
                    AtividadeAtualId = null,
                    TarefaAssociadas = new TarefaAssociada[]
                    {
                        new TarefaAssociada { KitId = 1, GrupoKit = 2},
                    }.ToList(),
                });
            }

            context.SaveChanges();
        }
    }
}
