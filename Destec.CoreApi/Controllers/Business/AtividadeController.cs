﻿using Destec.CoreApi.Models;
using Destec.CoreApi.Shared.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Destec.CoreApi.Models.Business;

namespace Destec.CoreApi.Controllers.Business
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AtividadeController : Controller
    {
        private readonly ApplicationDbContext db;
        private object next;

        public AtividadeController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpPost("execute")]
        public IActionResult Execute([FromQuery]string code)
        {
            var funcionario = db.Funcionarios
                                .Include(x => x.TarefaAssociadas)
                                .SingleOrDefault(x => x.Codigo == code);

            if (funcionario == null)
                return BadRequest("Código Inválido");

            return Ok(ExecuteAtividade(funcionario));
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

            DesalocaAtividades(funcionario);

            return Ok($"Atividades do usuário {funcionario.Nome} liberadas.");
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

            db.SaveChanges();
        }

        private Atividade GetAtividadeCorrente(Funcionario funcionario)
        {
            return db.Atividades
                        .Include(x => x.TipoAtividade).ThenInclude(x => x.Kit)
                        .SingleOrDefault(x => x.Id == funcionario.AtividadeAtualId);
        }

        private string ExecuteAtividade(Funcionario funcionario)
        {
            var atividadeCorrente = GetAtividadeCorrente(funcionario);

            // Se tem intervalo, contabiliza e continua a atividade
            if (atividadeCorrente != null && atividadeCorrente.IntervaloFrom.HasValue)
            {
                return FinalizaIntervalo(atividadeCorrente);
            }
            // Se tem parada, contabiliza e continua a atividade
            else if (atividadeCorrente != null && atividadeCorrente.ParadaFrom.HasValue)
            {
                return FinalizaParada(atividadeCorrente);
            }
            // Se tem atividade sem parada ou intervalo finaliza
            else if (atividadeCorrente != null)
            {
                FinalizaAtividadeCorrente(atividadeCorrente, funcionario);
            }

            return ProximaAtividade(funcionario);
        }

        private string ProximaAtividade(Funcionario funcionario)
        {
            // Busca a próxima atividade alocada
            var next = GetNextActivity(funcionario);

            if (next == null)
                return "Nenhuma atividade disponível para ser executada.";

            AlocaAtividades(funcionario, next);
            StartAtividade(funcionario, next);

            return $"{next.TipoAtividade.Kit.Nome} - {next.TipoAtividade.Nome}";
        }

        private void StartAtividade(Funcionario funcionario, Atividade next)
        {
            if (next.DataInicio == null)
                next.DataInicio = DateTime.Now;

            next.Status = AtividadeStatusEnum.EmExecucao;
            funcionario.AtividadeAtualId = next.Id;

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

        private void DesalocaAtividades(Funcionario funcionario)
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
                                  && t.Grupo == ta.GrupoKit
                                  && t.KitId == ta.KitId
                                  && ta.FuncionarioId == funcionario.Id
                              orderby a.PedidoItem.PedidoId, a.KitPedidoId, t.Ordem
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
            return $"{atividadeCorrente.TipoAtividade.Kit.Nome} - {atividadeCorrente.TipoAtividade.Nome}";
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
            return $"{atividadeCorrente.TipoAtividade.Kit.Nome} - {atividadeCorrente.TipoAtividade.Nome}";
        }

        private void FinalizaAtividadeCorrente(Atividade atividadeCorrente, Funcionario funcionario)
        {
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
    }
}