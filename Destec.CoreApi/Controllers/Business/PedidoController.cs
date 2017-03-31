using Destec.CoreApi.Models;
using Destec.CoreApi.Models.Business;
using Destec.CoreApi.Shared.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Destec.CoreApi.Controllers.Business
{
    [Route("api/[controller]")]
    public class PedidoController : Controller
    {
        private readonly ApplicationDbContext db;

        public PedidoController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string filter)
        {
            var result = db.Pedidos
                            .Include(x => x.Itens).ThenInclude(x => x.Kit)
                            .Where(x => string.IsNullOrEmpty(filter)
                                             || (!string.IsNullOrEmpty(x.Codigo) && x.Codigo.ContainsIgnoreNonSpacing(filter))
                                             || (!string.IsNullOrEmpty(x.Descricao) && x.Descricao.Contains(filter)))
                            .OrderBy(x => x.Id)
                            .ToList()
                            .Select(x => new Pedido
                            {
                                Id = x.Id,
                                Codigo = x.Codigo,
                                Descricao = x.Descricao,
                                Cancelado = x.Cancelado,
                                Prazo = x.Prazo,
                                Status = x.Status,
                                DataPedido = x.DataPedido,
                                Itens = x.Itens.Select(z => new PedidoItem
                                {
                                    Id = z.Id,
                                }).ToList(),
                            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = db.Pedidos
                            .Include(x => x.Itens).ThenInclude(x => x.Kit)
                            .SingleOrDefault(x => x.Id == id);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Pedido item)
        {
            foreach (var i in item.Itens)
            {
                i.Kit = null;
            }

            db.Pedidos.Add(item);
            db.SaveChanges();
            return Ok(db.Pedidos.Include(x => x.Itens).ThenInclude(x => x.Kit).Single(x => x.Id == item.Id));
        }

        [HttpGet("gerar/{id}")]
        public IActionResult GerarPedido(int id)
        {
            var pedido = db.Pedidos
                            .Include(x => x.Itens)
                                .ThenInclude(x => x.Kit)
                                .ThenInclude(x => x.TipoAtividades)
                            .Include(x => x.Itens)
                                .ThenInclude(x => x.Atividades)
                            .Single(x => x.Id == id);

            if (pedido.Itens.Any(x => x.Atividades.Count > 0) || pedido.Status != 0)
                return Ok(pedido);

            if (!pedido.Itens.Any())
                return BadRequest("Pedido sem itens");

            pedido.Status = StatusEnum.Gerando;
            db.SaveChanges();
            
            try
            {
                foreach (var item in pedido?.Itens)
                {
                    var SerialId = db.Atividades.Max(x => x.KitPedidoId);
                    var quantidadeKits = item.Quantidade ?? (item.QuantidadeApartamentosAndar * item.QuantidadeAndar) ?? throw new Exception("Quantidade do item não informado");

                    for (int i = 0; i < quantidadeKits; i++)
                    {
                        foreach (var atividade in item.Kit?.TipoAtividades?.Where(x => !x.Deleted).OrderBy(x => x.Grupo).ThenBy(x => x.Ordem))
                        {
                            db.Atividades.Add(new Atividade
                            {
                                PedidoItemId = item.Id,
                                TipoAtividadeId = atividade.Id,
                                KitPedidoId = i + SerialId,
                                KitNumero = GetKitNumero(item, i),
                                Status = AtividadeStatusEnum.Criada,
                            });
                        }
                        db.SaveChanges();
                    }
                }

                pedido.Status = StatusEnum.Gerado;
                db.SaveChanges();
            }
            catch (Exception)
            {
                pedido.Status = StatusEnum.Criado;
                db.SaveChanges();
            }

            return Ok(pedido);
        }

        [HttpGet("cancelar/{id}")]
        public IActionResult CancelarPedido(int id)
        {
            var pedido = db.Pedidos
                            .Include(x => x.Itens)
                                .ThenInclude(x => x.Kit)
                                .ThenInclude(x => x.TipoAtividades)
                            .Include(x => x.Itens)
                                .ThenInclude(x => x.Atividades)
                            .Single(x => x.Id == id);

            pedido.Status = StatusEnum.Cancelado;
            foreach (var item in pedido.Itens)
            {
                item.Cancelado = true;

                foreach (var atividade in item.Atividades.Where(x => !x.FuncionarioId.HasValue))
                {
                    atividade.Status = AtividadeStatusEnum.Cancelada;
                }
            }

            db.SaveChanges();

            return Ok(pedido);
        }

        [HttpPut]
        public IActionResult Put([FromBody] Pedido update)
        {
            var item = db.Pedidos
                        .Include(x => x.Itens)
                        .Single(x => x.Id == update.Id);

            item.Codigo = update.Codigo;
            item.Descricao = update.Descricao;
            item.Prazo = update.Prazo;
            item.DataPedido = update.DataPedido;
            item.Cancelado = update.Cancelado;

            foreach (var updateItem in update.Itens)
            {
                var newValue = item.Itens.SingleOrDefault(x => x.Id == updateItem.Id);

                if (newValue != null)
                {
                    newValue.KitId = updateItem.KitId;
                    newValue.Observacao = updateItem.Observacao;
                    newValue.Quantidade = updateItem.Quantidade;
                    newValue.PedidoId = updateItem.PedidoId;
                }
                else
                {
                    item.Itens.Add(new PedidoItem
                    {
                        KitId = updateItem.KitId,
                        PedidoId = updateItem.PedidoId,
                        Cancelado = false,
                        Quantidade = updateItem.Quantidade,
                        Observacao = updateItem.Observacao,
                    });
                }
            }

            db.SaveChanges();
            return Ok(db.Pedidos
                        .Include(x => x.Itens).ThenInclude(x => x.Kit)
                        .Single(x => x.Id == update.Id));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var item = db.Pedidos
                            .Include(x => x.Itens)
                                .ThenInclude(x => x.Kit)
                                .ThenInclude(x => x.TipoAtividades)
                .Single(x => x.Id == id);

            if (item.Itens.Any(x => x.Atividades?.Count > 0))
            {
                return BadRequest("Possui registros e não pode ser removido.");
            }
            else
            {
                db.Pedidos.Remove(item);
                db.SaveChanges();
                return Ok();
            }
        }

        /// <summary>
        /// Retorna o número do Kit
        /// </summary>
        /// <param name="item"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private string GetKitNumero(PedidoItem item, int i)
        {
            if (item.Quantidade.HasValue)
            {
                return (item.NumeroInicial + i).ToString();
            }
            else if (item.QuantidadeApartamentosAndar.HasValue && item.AndarInicial.HasValue)
            {
                var andar = (i / item.QuantidadeApartamentosAndar.Value) + item.AndarInicial.Value;
                var apto = i % item.QuantidadeApartamentosAndar.Value + 1;
                if (!string.IsNullOrEmpty(item.Bloco))
                    return $"BL{ item.Bloco }AP{ andar }{ apto.ToString("D2") }";
                else
                    return $"AP{ andar }{ apto.ToString("D2") }";
            }
            else
                return string.Empty;
        }
    }
}
