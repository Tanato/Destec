using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Destec.CoreApi.Models.Business;

namespace Destec.CoreApi.Models
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Atividade> Atividades { get; set; }
        public DbSet<TipoAtividade> TipoAtividades { get; set; }
        public DbSet<Kit> Kits { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoItem> PedidoItems { get; set; }
        public DbSet<TarefaAssociada> TarefaAssociadas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().ToTable("Users");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityRole>().ToTable("Roles");

            builder.Entity<Funcionario>().HasAlternateKey(x => x.Codigo);

            builder.Entity<Atividade>().HasIndex(x => x.Status);
            builder.Entity<Atividade>().HasIndex(x => x.FuncionarioId);
            builder.Entity<Atividade>().HasIndex(x => x.PedidoItemId);
            builder.Entity<Atividade>().HasIndex(x => x.TipoAtividadeId);
            builder.Entity<Atividade>().HasIndex(x => x.KitPedidoId);
        }
    }
}