using Destec.CoreApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Destec.CoreApi.Controllers.Business
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AtividadeController : Controller
    {
        private readonly ApplicationDbContext db;

        public AtividadeController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpPost("execute")]
        public IActionResult Execute([FromQuery]string code)
        {
            var method = HttpContext.Request.Method;

            var funcionario = db.Funcionarios
                                .Include(x => x.TarefaAssociadas)
                                .SingleOrDefault(x => x.Codigo == code);

            if (funcionario == null)
                return BadRequest("Código Inválido");

            // ToDo: If Exists, 
            if (funcionario.AtividadeAtualId.HasValue)
            {
                // ToDo: Get current activity
                var atividade = db.Atividades
                                    .Include(x => x.TipoAtividade).ThenInclude(x => x.Kit)
                                    .Single(x => x.Id == funcionario.AtividadeAtualId.Value);

                // If Has Interval Start
                if (atividade.IntervaloFrom.HasValue)
                {
                    // get difference from Now to StartInterval
                    var tspan = DateTime.Now.Subtract(atividade.IntervaloFrom.Value);

                    // Add to Timespan 
                    atividade.Intervalo = atividade.Intervalo == null ? tspan : atividade.Intervalo.Value.Add(tspan);

                    // Clean StartInterval Date
                    atividade.IntervaloFrom = null;

                    db.SaveChanges();
                    // Return Current Activity Information plus this interval span;
                    return Ok($"{atividade.TipoAtividade.Kit.Nome} - {atividade.TipoAtividade.Nome}");
                }
                else
                {
                    // set End Time
                    atividade.DataFinal = DateTime.Now;

                    // Clean Current activity of user
                    funcionario.AtividadeAtualId = null;
                }
            }

            // ToDo: Get Next Acitivity available for user
            var next = db.Atividades
                            .Include(x => x.TipoAtividade).ThenInclude(x => x.Kit)
                            .OrderByDescending(x => x.FuncionarioId).ThenBy(x => x.TipoAtividade.KitId).ThenBy(x => x.TipoAtividade.Ordem)
                            .FirstOrDefault(x => x.DataFinal == null && x.DataInicio == null
                                                && (x.FuncionarioId == funcionario.Id || x.FuncionarioId == null)
                                                && funcionario.TarefaAssociadas.Any(f => f.KitId == x.TipoAtividade.KitId
                                                                                    && f.GrupoKit == x.TipoAtividade.Grupo));

            if (next != null)
            {
                next.DataInicio = DateTime.Now;
                funcionario.AtividadeAtualId = next.Id;

                // Set User on Activity Group
                var group = db.Atividades
                                .Include(x => x.TipoAtividade)
                                .Where(x => next.PedidoItemId == x.PedidoItemId
                                            && next.TipoAtividade.Grupo == x.TipoAtividade.Grupo);

                // Set Acitivty Id on User
                foreach (var item in group)
                    item.FuncionarioId = funcionario.Id;

                db.SaveChanges();
                return Ok($"{next.TipoAtividade.Kit.Nome} - {next.TipoAtividade.Nome}");
            }
            else
            {
                return Ok("Nenhuma atividade para ser executada.");
            }

        }

        [HttpPost("interval")]
        public IActionResult Interval([FromQuery] string code)
        {
            var funcionario = db.Funcionarios.SingleOrDefault(x => x.Codigo.Equals(code));

            if (funcionario == null)
                return BadRequest("Código Inválido");

            // ToDo: If Exists, 
            if (funcionario.AtividadeAtualId.HasValue)
            {
                // ToDo: Get current activity
                var atividade = db.Atividades
                                    .Include(x => x.TipoAtividade).ThenInclude(x => x.Kit)
                                    .Single(x => x.Id == funcionario.AtividadeAtualId.Value);

                // If Has Interval Start
                if (atividade.IntervaloFrom.HasValue)
                {
                    // get difference from Now to StartInterval
                    var tspan = DateTime.Now.Subtract(atividade.IntervaloFrom.Value);

                    // Add to Timespan 
                    atividade.Intervalo = atividade.Intervalo == null ? tspan : atividade.Intervalo.Value.Add(tspan);

                    // Clean StartInterval Date
                    atividade.IntervaloFrom = null;

                    db.SaveChanges();
                    // Return Current Activity Information plus this interval span;
                    return Ok($"{atividade.TipoAtividade.Kit.Nome} - {atividade.TipoAtividade.Nome}");
                }
                else
                {
                    // set Start Interval DateTime
                    atividade.IntervaloFrom = DateTime.Now;
                    db.SaveChanges();

                    // Return Ok to user
                    return Ok("Intervalo iniciado.");
                }
            }
            else
            {
                return Ok("Nenhuma atividade em execução.");
            }
        }

        [HttpPost("stop")]
        public IActionResult Stop([FromQuery] string code)
        {
            var funcionario = db.Funcionarios.SingleOrDefault(x => x.Codigo.Equals(code));

            if (funcionario == null)
                return BadRequest("Código Inválido");

            // ToDo: If Exists, 
            if (funcionario.AtividadeAtualId.HasValue)
            {
                // ToDo: Get current activity
                var atividade = db.Atividades
                                    .Include(x => x.TipoAtividade)
                                    .Single(x => x.Id == funcionario.AtividadeAtualId.Value);
                // set End Time
                atividade.DataFinal = DateTime.Now;

                // Clean Current activity of user
                funcionario.AtividadeAtualId = null;

                db.SaveChanges();

                return Ok("Atividade finalizada.");
            }
            else
            {
                // Return "No Activity" message
                return Ok("Nenhuma atividade para finalizar.");
            }
        }
    }
}