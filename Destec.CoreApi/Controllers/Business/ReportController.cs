using Destec.CoreApi.Models;
using Destec.CoreApi.Models.Business;
using Destec.CoreApi.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Destec.CoreApi.Controllers.Business
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext db;

        public ReportController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        [Route("data.csv")]
        [Produces("text/csv")]
        public IActionResult GetDataAsCsv()
        {
            var query = db.Atividades
                        .Include(x => x.Funcionario)
                        .Include(x => x.PedidoItem).ThenInclude(x => x.Pedido)
                        .Include(x => x.TipoAtividade).ThenInclude(x => x.Kit)
                        .ToList();

            var result = query.OrderBy(x => x.PedidoItem.PedidoId).ThenBy(x => x.KitPedidoId).ThenBy(x => x.TipoAtividade.Grupo).ThenBy(x => x.TipoAtividade.Ordem)
                              .Select(x => new AtividadeReportViewModel
                              {
                                  Id = x.Id,
                                  TipoAtividade = x.TipoAtividade.Nome,
                                  Ordem = x.TipoAtividade.Ordem,
                                  Grupo = x.TipoAtividade.Grupo,
                                  Kit = x.TipoAtividade.Kit.Nome,
                                  PedidoId = x.PedidoItem.PedidoId,
                                  Pedido = x.PedidoItem.Pedido.Codigo,
                                  KitPedidoId = x.KitPedidoId,
                                  Status = EnumHelpers.GetEnumDescription(x.Status),
                                  TipoAtividadeId = x.TipoAtividadeId,
                                  
                                  FuncionarioId = x.FuncionarioId,
                                  Funcionario = x.Funcionario?.Nome,
                                  DataInicio = x.DataInicio,
                                  DataFinal = x.DataFinal,
                                  TempoFormatted = x.DataFinal?.Subtract(x.DataInicio.Value)
                                                        .Subtract(x.Intervalo ?? TimeSpan.FromTicks(0))
                                                        .Subtract(x.Parada ?? TimeSpan.FromTicks(0))
                                                        .ToString(@"hh\:mm\:ss"),
                                  Intervalo = x.Intervalo,
                                  IntervaloFormatted = x.Intervalo?.ToString(@"hh\:mm\:ss"),
                                  Parada = x.Parada,
                                  ParadaFormatted = x.Parada?.ToString(@"hh\:mm\:ss"),

                                  Ajudante = x.Ajudante?.Nome,
                                  AjudanteId = x.AjudanteId,
                                  Ajuda = x.Ajuda,
                                  AjudaFormatted = x.Ajuda?.ToString(@"hh\:mm\:ss"),
                              }).ToList();

            return Ok(result);
        }
    }
}
