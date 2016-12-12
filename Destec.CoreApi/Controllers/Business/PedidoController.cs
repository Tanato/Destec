using Destec.CoreApi.Models;
using Destec.CoreApi.Models.Business;
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
                            .Select(x => new Pedido
                            {
                                Id = x.Id,
                                Codigo = x.Codigo,
                                Descricao = x.Descricao,
                                Cancelado = x.Cancelado,
                                Prazo = x.Prazo,
                                DataPedido = x.DataPedido,
                                Itens = x.Itens.Select(z => new PedidoItem
                                {
                                    Id = z.Id,
                                }).ToList(),
                            })
                            .OrderBy(x => x.Id);

            return Ok(result.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = db.Pedidos
                            .Include(x => x.Itens).ThenInclude(x => x.Kit)
                            .Single(x => x.Id == id);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Pedido item)
        {
            db.Pedidos.Add(item);
            db.SaveChanges();
            return Ok(item);
        }

        [HttpGet("gerar/{id}")]
        public IActionResult GerarPedido([FromQuery] int id)
        {
            var pedido = db.Pedidos
                            .Include(x => x.Itens)
                                .ThenInclude(x => x.Kit)
                                .ThenInclude(x => x.TipoAtividades)
                            .Single(x => x.Id == id);

            foreach (var item in pedido?.Itens)
            {
                for (int i = 0; i < item.Quantidade; i++)
                {
                    foreach (var atividade in item.Kit?.TipoAtividades?.OrderBy(x => x.Ordem))
                    {
                        db.Atividades.Add(new Atividade
                        {
                            PedidoItemId = item.Id,
                            TipoAtividadeId = atividade.Id,
                            KitPedidoId = i,
                        });
                    }
                }
            }

            db.SaveChanges();
            return Ok();
        }

        [HttpPut]
        public IActionResult Put([FromBody] Pedido update)
        {
            var item = db.Pedidos
                        //.Include(x => x.Itens)
                        .Single(x => x.Id == update.Id);

            item.Codigo = update.Codigo;
            item.Descricao = update.Descricao;
            item.Prazo = update.Prazo;
            item.DataPedido = update.DataPedido;
            item.Cancelado = update.Cancelado;

            //foreach (var i in update.Itens)
            //{
            //    var atv = item.TipoAtividades.Single(x => x.Id == i.Id);
            //    atv.Nome = i.Nome;
            //    atv.Ordem = i.Ordem;
            //    atv.Pontos = i.Pontos;
            //    atv.Grupo = i.Grupo;
            //    atv.TempoEstimado = i.TempoEstimado;
            //}

            db.SaveChanges();
            return Ok(item);
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
    }
}
