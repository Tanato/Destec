using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Destec.CoreApi.Models;

namespace Destec.CoreApi.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20170304234311_v0.0.1")]
    partial class v001
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431");

            modelBuilder.Entity("Destec.CoreApi.Models.Business.Atividade", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<TimeSpan?>("Ajuda");

                    b.Property<DateTime?>("AjudaFrom");

                    b.Property<int?>("AjudanteId");

                    b.Property<DateTime?>("DataFinal");

                    b.Property<DateTime?>("DataInicio");

                    b.Property<int?>("FuncionarioId");

                    b.Property<TimeSpan?>("Intervalo");

                    b.Property<DateTime?>("IntervaloFrom");

                    b.Property<int>("KitPedidoId");

                    b.Property<TimeSpan?>("Parada");

                    b.Property<DateTime?>("ParadaFrom");

                    b.Property<int>("PedidoItemId");

                    b.Property<int>("Status");

                    b.Property<int>("TipoAtividadeId");

                    b.HasKey("Id");

                    b.HasIndex("AjudanteId");

                    b.HasIndex("FuncionarioId");

                    b.HasIndex("KitPedidoId");

                    b.HasIndex("PedidoItemId");

                    b.HasIndex("Status");

                    b.HasIndex("TipoAtividadeId");

                    b.ToTable("Atividades");
                });

            modelBuilder.Entity("Destec.CoreApi.Models.Business.Funcionario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AtividadeAjudaId");

                    b.Property<int?>("AtividadeAtualId");

                    b.Property<string>("Codigo")
                        .IsRequired();

                    b.Property<bool>("Inativo");

                    b.Property<string>("Nome");

                    b.HasKey("Id");

                    b.HasAlternateKey("Codigo");

                    b.ToTable("Funcionarios");
                });

            modelBuilder.Entity("Destec.CoreApi.Models.Business.Kit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Descricao");

                    b.Property<string>("ExternalCode");

                    b.Property<bool>("Inativo");

                    b.Property<string>("Nome");

                    b.HasKey("Id");

                    b.ToTable("Kits");
                });

            modelBuilder.Entity("Destec.CoreApi.Models.Business.Pedido", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Cancelado");

                    b.Property<string>("Codigo");

                    b.Property<DateTime?>("DataPedido");

                    b.Property<string>("Descricao");

                    b.Property<DateTime?>("Prazo");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.ToTable("Pedidos");
                });

            modelBuilder.Entity("Destec.CoreApi.Models.Business.PedidoItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Cancelado");

                    b.Property<int>("KitId");

                    b.Property<string>("Observacao");

                    b.Property<int>("PedidoId");

                    b.Property<int>("Quantidade");

                    b.HasKey("Id");

                    b.HasIndex("KitId");

                    b.HasIndex("PedidoId");

                    b.ToTable("PedidoItems");
                });

            modelBuilder.Entity("Destec.CoreApi.Models.Business.TarefaAssociada", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("FuncionarioId");

                    b.Property<int>("Grupo");

                    b.Property<int>("KitId");

                    b.HasKey("Id");

                    b.HasIndex("FuncionarioId");

                    b.HasIndex("KitId");

                    b.ToTable("TarefaAssociadas");
                });

            modelBuilder.Entity("Destec.CoreApi.Models.Business.TipoAtividade", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<short>("Grupo");

                    b.Property<int>("KitId");

                    b.Property<string>("Nome");

                    b.Property<short>("Ordem");

                    b.Property<decimal?>("Pontos");

                    b.Property<TimeSpan?>("TempoEstimado");

                    b.HasKey("Id");

                    b.HasIndex("KitId");

                    b.ToTable("TipoAtividades");
                });

            modelBuilder.Entity("Destec.CoreApi.Models.Log", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Description");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Log");
                });

            modelBuilder.Entity("Destec.CoreApi.Models.User", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<DateTime>("BirthDate");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("Inativo");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("Name");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("Password");

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasName("RoleNameIndex");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("UserTokens");
                });

            modelBuilder.Entity("Destec.CoreApi.Models.Business.Atividade", b =>
                {
                    b.HasOne("Destec.CoreApi.Models.Business.Funcionario", "Ajudante")
                        .WithMany()
                        .HasForeignKey("AjudanteId");

                    b.HasOne("Destec.CoreApi.Models.Business.Funcionario", "Funcionario")
                        .WithMany()
                        .HasForeignKey("FuncionarioId");

                    b.HasOne("Destec.CoreApi.Models.Business.PedidoItem", "PedidoItem")
                        .WithMany("Atividades")
                        .HasForeignKey("PedidoItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Destec.CoreApi.Models.Business.TipoAtividade", "TipoAtividade")
                        .WithMany("Atividades")
                        .HasForeignKey("TipoAtividadeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Destec.CoreApi.Models.Business.PedidoItem", b =>
                {
                    b.HasOne("Destec.CoreApi.Models.Business.Kit", "Kit")
                        .WithMany()
                        .HasForeignKey("KitId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Destec.CoreApi.Models.Business.Pedido", "Pedido")
                        .WithMany("Itens")
                        .HasForeignKey("PedidoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Destec.CoreApi.Models.Business.TarefaAssociada", b =>
                {
                    b.HasOne("Destec.CoreApi.Models.Business.Funcionario", "Funcionario")
                        .WithMany("TarefaAssociadas")
                        .HasForeignKey("FuncionarioId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Destec.CoreApi.Models.Business.Kit", "Kit")
                        .WithMany()
                        .HasForeignKey("KitId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Destec.CoreApi.Models.Business.TipoAtividade", b =>
                {
                    b.HasOne("Destec.CoreApi.Models.Business.Kit", "Kit")
                        .WithMany("TipoAtividades")
                        .HasForeignKey("KitId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Destec.CoreApi.Models.Log", b =>
                {
                    b.HasOne("Destec.CoreApi.Models.User", "User")
                        .WithMany("UserLogs")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Destec.CoreApi.Models.User")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Destec.CoreApi.Models.User")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Destec.CoreApi.Models.User")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
