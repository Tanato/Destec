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
    public class FuncionarioController : Controller
    {
        private readonly ApplicationDbContext db;

        public FuncionarioController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string filter)
        {
            var result = db.Funcionarios
                            .Include(x => x.TarefaAssociadas)
                            .Where(x => string.IsNullOrEmpty(filter)
                                             || (!string.IsNullOrEmpty(x.Nome) && x.Nome.ContainsIgnoreNonSpacing(filter))
                                             || (!string.IsNullOrEmpty(x.Codigo) && x.Codigo.Contains(filter)))
                            .Select(x => new Funcionario
                            {
                                Id = x.Id,
                                Nome = x.Nome,
                                Codigo = x.Codigo,
                                Inativo = x.Inativo,
                                TarefaAssociadas = x.TarefaAssociadas.Select(z => new TarefaAssociada
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
            var result = db.Funcionarios
                            .Include(x => x.TarefaAssociadas)
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
        public IActionResult Put([FromBody] Funcionario update)
        {
            var item = db.Funcionarios
                            .Include(x => x.TarefaAssociadas)
                        .Single(x => x.Id == update.Id);

            item.Nome = update.Nome;
            item.Codigo = update.Codigo;
            item.Inativo = update.Inativo;

            foreach (var i in update.TarefaAssociadas)
            {
                if (i.Id == 0)
                {
                    item.TarefaAssociadas.Add(new TarefaAssociada { KitId = i.KitId, GrupoKit = i.GrupoKit, FuncionarioId = update.Id });
                }
                else
                {
                    var atv = item.TarefaAssociadas.Single(x => x.Id == i.Id);
                    atv.KitId = i.KitId;
                    atv.GrupoKit = i.GrupoKit;
                }
            }

            db.SaveChanges();
            return Ok(item);
        }
    }
}
