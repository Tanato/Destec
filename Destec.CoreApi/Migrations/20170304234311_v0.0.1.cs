using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Destec.CoreApi.Migrations
{
    public partial class v001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Funcionarios",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    AtividadeAjudaId = table.Column<int>(nullable: true),
                    AtividadeAtualId = table.Column<int>(nullable: true),
                    Codigo = table.Column<string>(nullable: false),
                    Inativo = table.Column<bool>(nullable: false),
                    Nome = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funcionarios", x => x.Id);
                    table.UniqueConstraint("AK_Funcionarios_Codigo", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "Kits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    Descricao = table.Column<string>(nullable: true),
                    ExternalCode = table.Column<string>(nullable: true),
                    Inativo = table.Column<bool>(nullable: false),
                    Nome = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pedidos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    Cancelado = table.Column<bool>(nullable: false),
                    Codigo = table.Column<string>(nullable: true),
                    DataPedido = table.Column<DateTime>(nullable: true),
                    Descricao = table.Column<string>(nullable: true),
                    Prazo = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedidos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    BirthDate = table.Column<DateTime>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    Inativo = table.Column<bool>(nullable: false),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Password = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    SecurityStamp = table.Column<string>(nullable: true),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "TarefaAssociadas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    FuncionarioId = table.Column<int>(nullable: false),
                    Grupo = table.Column<int>(nullable: false),
                    KitId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TarefaAssociadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TarefaAssociadas_Funcionarios_FuncionarioId",
                        column: x => x.FuncionarioId,
                        principalTable: "Funcionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TarefaAssociadas_Kits_KitId",
                        column: x => x.KitId,
                        principalTable: "Kits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TipoAtividades",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    Grupo = table.Column<short>(nullable: false),
                    KitId = table.Column<int>(nullable: false),
                    Nome = table.Column<string>(nullable: true),
                    Ordem = table.Column<short>(nullable: false),
                    Pontos = table.Column<decimal>(nullable: true),
                    TempoEstimado = table.Column<TimeSpan>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoAtividades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TipoAtividades_Kits_KitId",
                        column: x => x.KitId,
                        principalTable: "Kits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PedidoItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    Cancelado = table.Column<bool>(nullable: false),
                    KitId = table.Column<int>(nullable: false),
                    Observacao = table.Column<string>(nullable: true),
                    PedidoId = table.Column<int>(nullable: false),
                    Quantidade = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidoItems_Kits_KitId",
                        column: x => x.KitId,
                        principalTable: "Kits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PedidoItems_Pedidos_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "Pedidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Log_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Atividades",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    Ajuda = table.Column<TimeSpan>(nullable: true),
                    AjudaFrom = table.Column<DateTime>(nullable: true),
                    AjudanteId = table.Column<int>(nullable: true),
                    DataFinal = table.Column<DateTime>(nullable: true),
                    DataInicio = table.Column<DateTime>(nullable: true),
                    FuncionarioId = table.Column<int>(nullable: true),
                    Intervalo = table.Column<TimeSpan>(nullable: true),
                    IntervaloFrom = table.Column<DateTime>(nullable: true),
                    KitPedidoId = table.Column<int>(nullable: false),
                    Parada = table.Column<TimeSpan>(nullable: true),
                    ParadaFrom = table.Column<DateTime>(nullable: true),
                    PedidoItemId = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    TipoAtividadeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atividades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Atividades_Funcionarios_AjudanteId",
                        column: x => x.AjudanteId,
                        principalTable: "Funcionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Atividades_Funcionarios_FuncionarioId",
                        column: x => x.FuncionarioId,
                        principalTable: "Funcionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Atividades_PedidoItems_PedidoItemId",
                        column: x => x.PedidoItemId,
                        principalTable: "PedidoItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Atividades_TipoAtividades_TipoAtividadeId",
                        column: x => x.TipoAtividadeId,
                        principalTable: "TipoAtividades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Atividades_AjudanteId",
                table: "Atividades",
                column: "AjudanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Atividades_FuncionarioId",
                table: "Atividades",
                column: "FuncionarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Atividades_KitPedidoId",
                table: "Atividades",
                column: "KitPedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_Atividades_PedidoItemId",
                table: "Atividades",
                column: "PedidoItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Atividades_Status",
                table: "Atividades",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Atividades_TipoAtividadeId",
                table: "Atividades",
                column: "TipoAtividadeId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItems_KitId",
                table: "PedidoItems",
                column: "KitId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItems_PedidoId",
                table: "PedidoItems",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_TarefaAssociadas_FuncionarioId",
                table: "TarefaAssociadas",
                column: "FuncionarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TarefaAssociadas_KitId",
                table: "TarefaAssociadas",
                column: "KitId");

            migrationBuilder.CreateIndex(
                name: "IX_TipoAtividades_KitId",
                table: "TipoAtividades",
                column: "KitId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_UserId",
                table: "Log",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Atividades");

            migrationBuilder.DropTable(
                name: "TarefaAssociadas");

            migrationBuilder.DropTable(
                name: "Log");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "PedidoItems");

            migrationBuilder.DropTable(
                name: "TipoAtividades");

            migrationBuilder.DropTable(
                name: "Funcionarios");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Pedidos");

            migrationBuilder.DropTable(
                name: "Kits");
        }
    }
}
