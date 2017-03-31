using Destec.CoreApi.Models;
using Destec.CoreApi.Shared.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Destec.CoreApi.Models.Business;
using Destec.CoreApi.Models.ViewModels;

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

        [HttpGet]
        public IActionResult GetAll([FromQuery] string filter, [FromQuery] IEnumerable<AtividadeStatusEnum> status)
        {
            if (status == null || status.Count() == 0)
                status = new List<AtividadeStatusEnum> { AtividadeStatusEnum.Criada, AtividadeStatusEnum.Alocada, AtividadeStatusEnum.EmExecucao };

            var query = db.Atividades
                        .Include(x => x.Funcionario)
                        .Include(x => x.PedidoItem).ThenInclude(x => x.Pedido)
                        .Include(x => x.TipoAtividade).ThenInclude(x => x.Kit)
                        .Where(x => status.Any(z => z == x.Status))
                        .ToList();

            foreach (var item in GetFilterList(filter))
            {
                query = query.Where(x => (x.Funcionario != null ? x.Funcionario.Nome.ContainsIgnoreNonSpacing(item) : false)
                                      || (!string.IsNullOrEmpty(x.PedidoItem.Pedido.Codigo) && x.PedidoItem.Pedido.Codigo.ContainsIgnoreNonSpacing(item))
                                      || (!string.IsNullOrEmpty(x.TipoAtividade.Nome) && x.TipoAtividade.Nome.ContainsIgnoreNonSpacing(item))
                                      || (!string.IsNullOrEmpty(x.TipoAtividade.Kit.Nome) && x.TipoAtividade.Kit.Nome.ContainsIgnoreNonSpacing(item))
                                      //|| (x.DataInicio != null && DateTime.TryParse(item, out var date) ? date.Date == x.DataInicio.Value.Date : false)
                                      || (x.Status.ToString() == item)).ToList();
            }

            var result = query.OrderBy(x => x.PedidoItem.PedidoId).ThenBy(x => x.KitPedidoId).ThenBy(x => x.TipoAtividade.Grupo).ThenBy(x => x.TipoAtividade.Ordem)
                              .Select(x => new AtividadeViewModel
                              {
                                  Id = x.Id,
                                  TipoAtividade = new TipoAtividade { Nome = x.TipoAtividade.Nome, Ordem = x.TipoAtividade.Ordem, Grupo = x.TipoAtividade.Grupo, Kit = new Kit { Nome = x.TipoAtividade.Kit.Nome } },
                                  Funcionario = new Funcionario { Nome = x.Funcionario?.Nome },
                                  PedidoItem = new PedidoItem { Pedido = new Pedido { Codigo = x.PedidoItem.Pedido.Codigo } },
                                  DataInicio = x.DataInicio,
                                  DataFinal = x.DataFinal,
                                  FuncionarioId = x.FuncionarioId,
                                  Intervalo = x.Intervalo,
                                  KitPedidoId = x.KitPedidoId,
                                  KitNumero = x.KitNumero,
                                  Status = x.Status,
                                  StatusDescricao = EnumHelpers.GetEnumDescription(x.Status),
                                  PedidoItemId = x.PedidoItemId,
                                  TipoAtividadeId = x.TipoAtividadeId,
                                  Parada = x.Parada,
                                  IntervaloFormatted = x.Intervalo?.ToString(@"hh\:mm\:ss"),
                                  TempoFormatted = x.DataFinal?.Subtract(x.DataInicio.Value)
                                                        .Subtract(x.Intervalo ?? TimeSpan.FromTicks(0))
                                                        .Subtract(x.Parada ?? TimeSpan.FromTicks(0))
                                                        .ToString(@"hh\:mm\:ss"),
                              }).ToList();

            return Ok(result);
        }

        [HttpGet("execucao")]
        public IActionResult GetExecucao()
        {
            var result = db.Atividades
                            .Include(x => x.Funcionario)
                            .Include(x => x.Ajudante)
                            .Include(x => x.PedidoItem).ThenInclude(x => x.Pedido)
                            .Include(x => x.TipoAtividade).ThenInclude(x => x.Kit)
                            .Where(x => x.Status == AtividadeStatusEnum.EmExecucao)
                            .OrderBy(x => x.DataInicio)
                            .ToList()
                            .Select(x => new AtividadeViewModel
                            {
                                Id = x.Id,
                                PedidoItemId = x.PedidoItemId,
                                PedidoItem = new PedidoItem { Pedido = new Pedido { Codigo = x.PedidoItem.Pedido.Codigo } },
                                KitPedidoId = x.KitPedidoId,
                                KitNumero = x.KitNumero,

                                Status = x.Status,
                                StatusDescricao = EnumHelpers.GetEnumDescription(x.Status),

                                TipoAtividadeId = x.TipoAtividadeId,
                                TipoAtividade = new TipoAtividade
                                {
                                    Nome = x.TipoAtividade.Nome,
                                    Ordem = x.TipoAtividade.Ordem,
                                    Grupo = x.TipoAtividade.Grupo,
                                    Kit = new Kit { Nome = x.TipoAtividade.Kit.Nome },
                                    TempoEstimado = x.TipoAtividade.TempoEstimado,
                                    Pontos = x.TipoAtividade.Pontos,
                                },

                                FuncionarioId = x.FuncionarioId,
                                Funcionario = new Funcionario { Nome = x.Funcionario?.Nome },

                                AjudanteId = x.AjudanteId,
                                Ajudante = new Funcionario { Nome = x.Ajudante?.Nome },

                                InAjuda = x.AjudaFrom.HasValue,

                                InParada = x.ParadaFrom.HasValue,
                                Parada = x.Parada,

                                InIntervalo = x.IntervaloFrom.HasValue,

                                DataInicio = x.DataInicio,
                                TempoCorrente = (x.IntervaloFrom ?? x.ParadaFrom ?? DateTime.Now).Subtract(x.DataInicio.Value)
                                                            .Subtract(x.Intervalo ?? TimeSpan.FromTicks(0))
                                                            .Subtract(x.Parada ?? TimeSpan.FromTicks(0)).TotalMilliseconds,

                                Intervalo = x.Intervalo,
                                IntervaloCorrente = DateTime.Now.Subtract(x.IntervaloFrom ?? DateTime.Now)
                                                                .Add(x.Intervalo ?? TimeSpan.FromTicks(0)).TotalMilliseconds,
                            }).ToList();

            return Ok(result);
        }

        [HttpPost("execute")]
        public IActionResult Execute([FromQuery] string code)
        {
            var funcionario = db.Funcionarios
                                .Include(x => x.TarefaAssociadas)
                                .SingleOrDefault(x => x.Codigo == code);

            if (funcionario == null)
                return BadRequest("Código Inválido");

            return Ok(ExecuteAtividade(funcionario));
        }

        [HttpGet("help")]
        public IActionResult GetHelp([FromQuery] string code)
        {
            var funcionario = db.Funcionarios
                                .Include(x => x.TarefaAssociadas)
                                .SingleOrDefault(x => x.Codigo == code);

            if (funcionario == null)
                return BadRequest("Código Inválido");

            var atividade = GetAtividadeCorrente(funcionario);

            return Ok($"{ atividade.KitPedidoId }: { atividade.TipoAtividade.Kit.Nome }{ Environment.NewLine }{ atividade.TipoAtividade.Nome }");
        }

        [HttpPost("help")]
        public IActionResult PostHelp([FromQuery] string code, [FromQuery] string ajudaCode)
        {
            if (code == ajudaCode)
                return BadRequest("Ajudante não pode ter o mesmo código do Ajudado");

            var ajudado = db.Funcionarios.SingleOrDefault(x => x.Codigo == ajudaCode);

            var ajudante = db.Funcionarios
                                .Include(x => x.TarefaAssociadas)
                                .SingleOrDefault(x => x.Codigo == code);

            var atividadeAjuda = db.Atividades.SingleOrDefault(x => x.Id == ajudado.AtividadeAtualId);

            if (atividadeAjuda.AjudaFrom.HasValue || atividadeAjuda.AjudanteId.HasValue)
                return BadRequest("Atividade já possui ajudante.");

            if (ajudante == null || ajudado == null)
                return BadRequest("Código Inválido");

            if (!ajudado.AtividadeAtualId.HasValue || atividadeAjuda == null)
                return BadRequest("Funcionário sem atividade para ser ajudado");

            ExecuteAtividade(ajudante, true);

            StartAjuda(ajudante, atividadeAjuda);

            return Ok("Ajuda iniciada.");
        }

        [HttpPost("interval")]
        public IActionResult Interval([FromQuery] string code)
        {
            var funcionario = db.Funcionarios.SingleOrDefault(x => x.Codigo.Equals(code));

            if (funcionario == null)
                return BadRequest("Código Inválido");

            var atividade = GetAtividadeCorrente(funcionario);

            if (atividade == null)
                return Ok("Nenhuma atividade em execução.");

            // Inicia se não houver intervalo
            if (!atividade.IntervaloFrom.HasValue)
                return Ok(IniciaIntervalo(atividade));
            else
                return Ok(FinalizaIntervalo(atividade));
        }

        [HttpPost("stop")]
        public IActionResult Stop([FromQuery] string code)
        {
            var funcionario = db.Funcionarios.SingleOrDefault(x => x.Codigo.Equals(code));

            if (funcionario == null)
                return BadRequest("Código Inválido");

            var atividade = GetAtividadeCorrente(funcionario);

            if (atividade == null)
                return Ok("Nenhuma atividade em execução.");

            // Inicia se não houver parada
            if (!atividade.ParadaFrom.HasValue)
                return Ok(IniciaParada(atividade));
            else
                return Ok(FinalizaParada(atividade));
        }

        [HttpPost("deallocate")]
        public IActionResult Deallocate([FromQuery] string code)
        {
            var funcionario = db.Funcionarios.SingleOrDefault(x => x.Codigo.Equals(code));

            if (funcionario == null)
                return BadRequest("Código Inválido");

            ResetAtividade(funcionario);

            DesalocaAtividadesFuncionario(funcionario);

            return Ok($"Atividades do usuário {funcionario.Nome} liberadas.");
        }

        [HttpGet("deallocateactivity/{activityId}")]
        public IActionResult DeallocateActivity(int activityId)
        {
            var activity = db.Atividades
                             .Include(x => x.TipoAtividade)
                             .Include(x => x.Funcionario)
                             .SingleOrDefault(x => x.Id == activityId
                                                && (x.Status == AtividadeStatusEnum.EmExecucao || x.Status == AtividadeStatusEnum.Alocada));

            if (activity == null)
                return BadRequest("Identificador Inválido");

            DesalocaAtividadesGrupo(activity);

            return Ok($"Atividades do usuário liberadas.");
        }

        [HttpPost("manualallocate")]
        public IActionResult ManualAllocate([FromBody] List<Atividade> atividades)
        {
            if (atividades != null)
            {
                foreach (var item in atividades.GroupBy(x => new { x.KitPedidoId, x.TipoAtividade.Grupo }))
                {
                    var funcionario = db.Funcionarios.Single(x => x.Id == item.First().FuncionarioId);

                    if (funcionario != null)
                        AlocaAtividades(funcionario, item.First());
                }
            }

            return Ok();
        }

        private void DesalocaAtividadesGrupo(Atividade activity)
        {
            var activityGroup = db.Atividades
                                  .Include(x => x.TipoAtividade)
                                  .Where(x => x.PedidoItemId == activity.PedidoItemId
                                           && x.KitPedidoId == activity.KitPedidoId
                                           && x.TipoAtividade.Grupo == activity.TipoAtividade.Grupo
                                           && (x.Status == AtividadeStatusEnum.EmExecucao || x.Status == AtividadeStatusEnum.Alocada));

            if (activityGroup.Any(x => x.Status == AtividadeStatusEnum.EmExecucao))
                ResetAtividade(activity.Funcionario);

            // Set Acitivty Id on User
            foreach (var item in activityGroup)
            {
                item.Status = AtividadeStatusEnum.Criada;
                item.FuncionarioId = null;
            }

            db.SaveChanges();
        }

        private void ResetAtividade(Funcionario funcionario)
        {
            var atividade = GetAtividadeCorrente(funcionario);

            atividade.Intervalo = null;
            atividade.IntervaloFrom = null;
            atividade.Parada = null;
            atividade.ParadaFrom = null;
            atividade.FuncionarioId = null;
            atividade.DataInicio = null;
            atividade.Status = AtividadeStatusEnum.Criada;

            funcionario.AtividadeAtualId = null;

            db.SaveChanges();
        }

        private Atividade GetAtividadeCorrente(Funcionario funcionario)
        {
            return db.Atividades
                        .Include(x => x.TipoAtividade).ThenInclude(x => x.Kit)
                        .SingleOrDefault(x => x.Id == funcionario.AtividadeAtualId);
        }

        private Atividade GetAjudaCorrente(Funcionario funcionario)
        {
            return db.Atividades
                        .Include(x => x.TipoAtividade).ThenInclude(x => x.Kit)
                        .SingleOrDefault(x => x.Id == funcionario.AtividadeAjudaId);
        }

        private string ExecuteAtividade(Funcionario funcionario, bool ajuda = false)
        {
            var mensagem = string.Empty;
            var atividadeCorrente = GetAtividadeCorrente(funcionario);

            // Se não tem atividade alocada
            if (atividadeCorrente == null && !ajuda)
            {
                // Verifica se está em ajuda
                var atividadeAjuda = GetAjudaCorrente(funcionario);
                if (atividadeAjuda != null)
                    FinalizaAjuda(atividadeAjuda);

                mensagem = ProximaAtividade(funcionario);
            }
            // Se tem ajuda, contabiliza e continua a atividade
            if (atividadeCorrente != null && atividadeCorrente.AjudaFrom.HasValue)
            {
                FinalizaAjuda(atividadeCorrente);

                mensagem = ProximaAtividade(funcionario);
            }
            // Se tem intervalo, contabiliza e continua a atividade
            if (atividadeCorrente != null && atividadeCorrente.IntervaloFrom.HasValue)
            {
                mensagem = FinalizaIntervalo(atividadeCorrente);

                if (ajuda)
                {
                    FinalizaAtividadeEPedido(funcionario, atividadeCorrente);
                    return string.Empty;
                }
            }
            // Se tem parada, contabiliza e continua a atividade
            else if (atividadeCorrente != null && atividadeCorrente.ParadaFrom.HasValue)
            {
                mensagem = FinalizaParada(atividadeCorrente);

                if (ajuda)
                {
                    FinalizaAtividadeEPedido(funcionario, atividadeCorrente);
                    return string.Empty;
                }
            }
            // Se tem atividade sem parada ou intervalo finaliza
            else if (atividadeCorrente != null)
            {
                FinalizaAtividadeEPedido(funcionario, atividadeCorrente);

                if (ajuda)
                    return string.Empty;

                mensagem = ProximaAtividade(funcionario);
            }

            return mensagem;
        }

        private void FinalizaAtividadeEPedido(Funcionario funcionario, Atividade atividadeCorrente)
        {
            FinalizaAtividadeCorrente(atividadeCorrente, funcionario);

            FinalizaPedido(atividadeCorrente);
        }

        private void FinalizaPedido(Atividade atividadeCorrente)
        {
            var pedido = db.Pedidos.Include(x => x.Itens).ThenInclude(x => x.Atividades).Single(x => x.Itens.Any(z => z.PedidoId == atividadeCorrente.PedidoItemId));
            if (pedido.Itens.All(x => x.Atividades.All(z => z.Status == AtividadeStatusEnum.Finalizada)))
            {
                pedido.Status = StatusEnum.Finalizado;
            }
        }

        private string ProximaAtividade(Funcionario funcionario)
        {
            // Busca a próxima atividade alocada
            var next = GetNextActivity(funcionario);

            if (next == null)
                return "Nenhuma atividade disponível para ser executada.";

            AlocaAtividades(funcionario, next);
            StartAtividade(funcionario, next);

            return $"{ next.KitPedidoId }: { next.TipoAtividade.Kit.Nome }{ Environment.NewLine }{ next.TipoAtividade.Nome }";
        }

        private void StartAtividade(Funcionario funcionario, Atividade next)
        {
            if (next.DataInicio == null)
                next.DataInicio = DateTime.Now;

            next.Status = AtividadeStatusEnum.EmExecucao;
            funcionario.AtividadeAtualId = next.Id;

            db.SaveChanges();
        }

        private void StartAjuda(Funcionario funcionario, Atividade ajuda)
        {
            ajuda.AjudaFrom = DateTime.Now;
            ajuda.AjudanteId = funcionario.Id;
            funcionario.AtividadeAjudaId = ajuda.Id;

            db.SaveChanges();
        }

        private void AlocaAtividades(Funcionario funcionario, Atividade next)
        {
            // Set User on Activity Group
            var group = db.Atividades
                            .Include(x => x.TipoAtividade)
                            .Where(x => next.PedidoItemId == x.PedidoItemId
                                        && next.TipoAtividade.Grupo == x.TipoAtividade.Grupo
                                        && next.KitPedidoId == x.KitPedidoId
                                        && x.Status == AtividadeStatusEnum.Criada
                                        && x.FuncionarioId != funcionario.Id);

            // Set Acitivty Id on User
            foreach (var item in group)
            {
                item.Status = AtividadeStatusEnum.Alocada;
                item.FuncionarioId = funcionario.Id;
            }

            db.SaveChanges();
        }

        private void DesalocaAtividadesFuncionario(Funcionario funcionario)
        {
            // Set User on Activity Group
            var group = db.Atividades
                            .Include(x => x.TipoAtividade)
                            .Where(x => x.Status == AtividadeStatusEnum.Alocada
                                        && x.FuncionarioId != funcionario.Id);

            // Set Acitivty Id on User
            foreach (var item in group)
            {
                item.Status = AtividadeStatusEnum.Criada;
                item.FuncionarioId = null;
            }

            db.SaveChanges();
        }

        private Atividade GetNextActivity(Funcionario funcionario)
        {
            // Busca a próxima alocada
            var next = db.Atividades
                            .Include(x => x.TipoAtividade).ThenInclude(x => x.Kit)
                            .OrderBy(x => x.TipoAtividade.Ordem)
                            .FirstOrDefault(x => x.FuncionarioId == funcionario.Id
                                                && (x.Status == AtividadeStatusEnum.Alocada || x.Status == AtividadeStatusEnum.EmExecucao));

            // Se não tiver atividade alocada, busca a próxima da fila.
            if (next == null)
            {
                var nextId = (from a in db.Atividades
                              join t in db.TipoAtividades on a.TipoAtividadeId equals t.Id
                              join p in db.PedidoItems on a.PedidoItemId equals p.Id
                              from ta in db.TarefaAssociadas
                              where a.FuncionarioId == null
                                  && a.Status == AtividadeStatusEnum.Criada
                                  && t.Grupo == ta.Grupo
                                  && t.KitId == ta.KitId
                                  && ta.FuncionarioId == funcionario.Id
                              orderby a.PedidoItem.PedidoId, a.KitPedidoId, t.Grupo, t.Ordem
                              select a.Id).FirstOrDefault();

                if (nextId != 0)
                {
                    next = db.Atividades
                            .Include(x => x.TipoAtividade).ThenInclude(x => x.Kit)
                            .Single(x => x.Id == nextId);
                }
            }

            return next;
        }

        private string FinalizaIntervalo(Atividade atividadeCorrente)
        {
            // get difference from Now to StartInterval
            var tspan = DateTime.Now.Subtract(atividadeCorrente.IntervaloFrom.Value);

            // Add to Timespan 
            atividadeCorrente.Intervalo = atividadeCorrente.Intervalo == null ? tspan : atividadeCorrente.Intervalo.Value.Add(tspan);

            // Clean StartInterval Date
            atividadeCorrente.IntervaloFrom = null;

            db.SaveChanges();
            // Return Current Activity Information plus this interval span;
            return $"{ atividadeCorrente.KitPedidoId }: { atividadeCorrente.TipoAtividade.Kit.Nome }{ Environment.NewLine }{ atividadeCorrente.TipoAtividade.Nome }";
        }

        private string FinalizaParada(Atividade atividadeCorrente)
        {
            // get difference from Now to StartInterval
            var tspan = DateTime.Now.Subtract(atividadeCorrente.ParadaFrom.Value);

            // Add to Timespan 
            atividadeCorrente.Parada = atividadeCorrente.Parada == null ? tspan : atividadeCorrente.Parada.Value.Add(tspan);

            // Clean StartInterval Date
            atividadeCorrente.ParadaFrom = null;

            db.SaveChanges();
            // Return Current Activity Information plus this interval span;
            return $"{ atividadeCorrente.KitPedidoId }: { atividadeCorrente.TipoAtividade.Kit.Nome }{ Environment.NewLine }{ atividadeCorrente.TipoAtividade.Nome }";
        }

        private void FinalizaAjuda(Atividade atividadeCorrente)
        {
            // get difference from Now to StartInterval
            var tspan = DateTime.Now.Subtract(atividadeCorrente.AjudaFrom.Value);

            // Add to Timespan 
            atividadeCorrente.Ajuda = atividadeCorrente.Ajuda == null ? tspan : atividadeCorrente.Ajuda.Value.Add(tspan);

            // Clean StartInterval Date
            atividadeCorrente.AjudaFrom = null;

            var ajudante = db.Funcionarios.Single(x => x.Id == atividadeCorrente.AjudanteId);
            ajudante.AtividadeAjudaId = null;

            db.SaveChanges();

            return;
        }

        private void FinalizaAtividadeCorrente(Atividade atividadeCorrente, Funcionario funcionario)
        {
            if (atividadeCorrente.AjudaFrom.HasValue)
                FinalizaAjuda(atividadeCorrente);

            atividadeCorrente.Status = AtividadeStatusEnum.Finalizada;
            atividadeCorrente.DataFinal = DateTime.Now;
            funcionario.AtividadeAtualId = null;

            db.SaveChanges();
        }

        private object IniciaIntervalo(Atividade atividade)
        {
            if (atividade.ParadaFrom.HasValue)
                FinalizaParada(atividade);

            // set Start Interval DateTime
            atividade.IntervaloFrom = DateTime.Now;
            db.SaveChanges();

            // Return Ok to user
            return "Intervalo iniciado.";
        }

        private object IniciaParada(Atividade atividade)
        {
            if (atividade.IntervaloFrom.HasValue)
                FinalizaIntervalo(atividade);

            // set Start Interval DateTime
            atividade.ParadaFrom = DateTime.Now;
            db.SaveChanges();

            // Return Ok to user
            return "Intervalo iniciado.";
        }

        private static List<string> GetFilterList(string filter)
        {
            List<string> filterList = new List<string>();
            if (!string.IsNullOrEmpty(filter))
            {
                foreach (var item in filter.Split(',', ';').Select(x => x.Trim()))
                {
                    var e = Enum.GetValues(typeof(AtividadeStatusEnum))
                                       .Cast<AtividadeStatusEnum>()
                                       .Where(x => x.GetEnumDescription().ContainsIgnoreNonSpacing(item))
                                       .Select(x => x.ToString());
                    if (e.Count() > 0)
                        filterList.AddRange(e);
                    else
                        filterList.Add(item);
                }
            }

            return filterList;
        }
    }
}