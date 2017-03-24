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
                                TipoAtividades = x.TipoAtividades.Where(t => !t.Deleted).Select(z => new TipoAtividade
                                {
                                    Id = z.Id,
                                }).ToList(),
                            })
                            .OrderBy(x => x.Id);

            return Ok(result.ToList());
        }

        [HttpGet]
        [Route("gruposelect")]
        public IActionResult GetSelect([FromQuery] int filter = 0)
        {
            var result = db.TipoAtividades
                            .Where(x => filter == 0 || x.KitId == filter)
                            .Select(x => x.Grupo)
                            .Distinct()
                            .OrderBy(x => x);

            return Ok(result.ToList());
        }

        [HttpGet]
        [Route("kitselect")]
        public IActionResult GetSelect()
        {
            var result = db.Kits
                            .Where(x => !x.Inativo)
                            .Select(x => new Kit
                            {
                                Id = x.Id,
                                Nome = x.Nome,
                            })
                            .OrderBy(x => x.Id);

            return Ok(result.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = db.Kits
                            .Include(x => x.TipoAtividades)
                            .Select(s => new Kit
                            {
                                Id = s.Id,
                                Descricao = s.Descricao,
                                ExternalCode = s.ExternalCode,
                                Nome = s.Nome,
                                TipoAtividades = s.TipoAtividades.Where(t => !t.Deleted).OrderBy(x => x.Grupo).ThenBy(x => x.Ordem).ToList(),
                                Inativo = s.Inativo
                            })
                            .SingleOrDefault(x => x.Id == id);

            return Ok(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Kit model)
        {
            if (model != null && model.Id == 0)
            {
                db.Kits.Add(model);
                db.SaveChanges();
            }
            return Ok(model);
        }

        [HttpPut]
        public IActionResult Put([FromBody] Kit model)
        {
            if (model != null)
            {
                var item = db.Kits
                            .Include(x => x.TipoAtividades)
                            .Single(x => x.Id == model.Id);

                item.Nome = model.Nome;
                item.Descricao = model.Descricao;
                item.Inativo = model.Inativo;

                foreach (var i in model.TipoAtividades)
                {
                    var atv = item.TipoAtividades.SingleOrDefault(x => x.Id == i.Id);
                    if (atv != null)
                    {
                        atv.Nome = i.Nome;
                        atv.Ordem = i.Ordem;
                        atv.Pontos = i.Pontos;
                        atv.Grupo = i.Grupo;
                        atv.TempoEstimado = i.TempoEstimado;
                        atv.Deleted = atv.Deleted;
                    }
                }

                var atividadesAdicionar = model.TipoAtividades.Where(x => x.Id == 0);
                item.TipoAtividades.AddRange(atividadesAdicionar);

                foreach (var atv in item.TipoAtividades.Where(x => !model.TipoAtividades.Any(z => z.Id == x.Id)))
                    atv.Deleted = true;

                db.SaveChanges();
                
                AtualizaPedidos(model);

                var result = db.Kits
                            .Include(x => x.TipoAtividades)
                            .Single(x => x.Id == model.Id);
                return Ok(model);
            }
            else
                return BadRequest("model is null");
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

        private void AtualizaPedidos(Kit kit)
        {
            var pedidoItens = db.PedidoItems
                            .Include(x => x.Pedido)
                            .Include(x => x.Atividades)
                            .Where(x => x.Pedido.Status == StatusEnum.Gerado
                                       && x.KitId == kit.Id
                                       && x.Atividades.Any(a => a.Status == AtividadeStatusEnum.Criada
                                                             || a.Status == AtividadeStatusEnum.Alocada));
            foreach (var item in pedidoItens)
            {
                foreach (var pedidoKit in item.Atividades.GroupBy(x => x.KitPedidoId))
                {
                    if (pedidoKit.Any() && pedidoKit.All(x => x.Status == AtividadeStatusEnum.Criada || x.Status == AtividadeStatusEnum.Alocada))
                    {
                        var atividadeInicialKit = pedidoKit.First();

                        foreach (var atividadeKit in kit.TipoAtividades.Where(x => !x.Deleted).OrderBy(x => x.Grupo).ThenBy(x => x.Ordem))
                        {
                            var atv = pedidoKit.SingleOrDefault(x => x.TipoAtividadeId == atividadeKit.Id);
                            if (atv == null)
                            {
                                db.Atividades.Add(new Atividade
                                {
                                    PedidoItemId = atividadeInicialKit.PedidoItemId,
                                    TipoAtividadeId = atividadeKit.Id,
                                    KitPedidoId = atividadeInicialKit.KitPedidoId,
                                    Status = AtividadeStatusEnum.Criada,
                                });
                            }
                        }

                        // Remove todas as atividades que foram exluidas do kit;
                        item.Atividades.RemoveAll(x => !kit.TipoAtividades.Any(z => z.Id == x.TipoAtividadeId) && x.KitPedidoId == atividadeInicialKit.KitPedidoId);

                    }
                }
                db.SaveChanges();
            }
        }
    }
}
