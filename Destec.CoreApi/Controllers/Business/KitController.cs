using Destec.CoreApi.Models;
using Destec.CoreApi.Models.Business;
using Destec.CoreApi.Shared.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Destec.CoreApi.Controllers.Business
{
    [Route("api/[controller]")]
    public class KitController : Controller
    {
        private readonly ApplicationDbContext db;

        public KitController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string filter)
        {
            var result = db.Kits
                            .Include(x => x.TipoAtividades)
                            .Where(x => string.IsNullOrEmpty(filter)
                                             || (!string.IsNullOrEmpty(x.Nome) && x.Nome.ContainsIgnoreNonSpacing(filter))
                                             || (!string.IsNullOrEmpty(x.Descricao) && x.Descricao.Contains(filter)))
                            .Select(x => new Kit
                            {
                                Id = x.Id,
                                Nome = x.Nome,
                                Descricao = x.Descricao,
                                Inativo = x.Inativo,
                                TipoAtividades = x.TipoAtividades.Select(z => new TipoAtividade
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
            var result = db.Kits
                            .Include(x => x.TipoAtividades)
                            .Single(x => x.Id == id);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Kit item)
        {
            db.Kits.Add(item);
            db.SaveChanges();
            return Ok(item);
        }

        [HttpPut]
        public IActionResult Put([FromBody] Kit update)
        {
            var item = db.Kits
                        .Include(x => x.TipoAtividades)
                        .Single(x => x.Id == update.Id);

            item.Nome = update.Nome;
            item.Descricao = update.Descricao;
            item.Inativo = update.Inativo;

            foreach (var i in update.TipoAtividades)
            {
                var atv = item.TipoAtividades.Single(x => x.Id == i.Id);
                atv.Nome = i.Nome;
                atv.Ordem = i.Ordem;
                atv.Pontos = i.Pontos;
                atv.Grupo = i.Grupo;
                atv.TempoEstimado = i.TempoEstimado;
            }

            db.SaveChanges();
            return Ok(item);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var item = db.Kits
                .Include(x => x.TipoAtividades).ThenInclude(x => x.Atividades)
                .Single(x => x.Id == id);

            if (item.TipoAtividades.Any(x => x.Atividades?.Count > 0))
            {
                return BadRequest("Possui registros e não pode ser removido.");
            }
            else
            {
                db.Kits.Remove(item);
                db.SaveChanges();
                return Ok();
            }
        }

        [HttpGet("gerar/{id}")]
        public IActionResult GerarPedido(int id)
        {
            var atividades = db.TipoAtividades
                            .OrderBy(x => x.Ordem)
                            .Where(x => x.KitId == id);

            var pedidos = new List<Pedido>();

            for (int i = 0; i < 10000; i++)
            {
                pedidos.Add(new Pedido
                {
                    Itens = new PedidoItem[]
                    {
                    new PedidoItem {
                        KitId = id,
                        Atividades = atividades.Select(atividade => new Atividade
                                                                        {
                                                                            TipoAtividadeId = atividade.Id,
                                                                            KitPedidoId = 1,
                                                                            Status = AtividadeStatusEnum.Criada,
                                                                        }).ToList(),
                    }
                    }.ToList(),
                });
            }

            var m = atividades.Count() * 100;

            for (int i = 0; i < m; i++)
            {
                db.Pedidos.AddRange(pedidos.Skip(i * m).Take(m));
                db.SaveChanges();
            }

            return Ok();
        }
    }
}
