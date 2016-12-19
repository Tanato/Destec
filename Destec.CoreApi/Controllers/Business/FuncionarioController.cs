using Destec.CoreApi.Models;
using Destec.CoreApi.Models.Business;
using Destec.CoreApi.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Destec.CoreApi.Controllers.Business
{
    [Route("api/[controller]")]
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
                                    KitId = z.KitId,
                                    Grupo = z.Grupo,
                                }).ToList(),
                            })
                            .OrderBy(x => x.Id);

            return Ok(result.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = db.Funcionarios
                            .Include(x => x.TarefaAssociadas).ThenInclude(x => x.Kit)
                            .Single(x => x.Id == id);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Funcionario item)
        {
            if (item.Codigo == string.Empty)
            {
                item.Codigo = GenerateCode(db.Funcionarios.Select(x => x.Codigo));
            }
            else if (db.Funcionarios.Any(x => x.Codigo == item.Codigo))
            {
                return BadRequest("Código já existente, favor criar um novo.");
            }

            foreach (var i in item.TarefaAssociadas)
            {
                i.Kit = null;
                i.Funcionario = null;
            }

            db.Funcionarios.Add(item);
            db.SaveChanges();
            return Ok(item);
        }

        [HttpPut]
        public IActionResult Put([FromBody] FuncionarioViewModel update)
        {
            var item = db.Funcionarios
                            .Include(x => x.TarefaAssociadas)
                        .Single(x => x.Id == update.Id);

            item.Nome = update.Nome;
            item.Inativo = update.Inativo;


            if (update.TarefaAssociadas != null)
                foreach (var i in update.TarefaAssociadas)
                {
                    if (i.Id == 0)
                    {
                        item.TarefaAssociadas.Add(new TarefaAssociada { KitId = i.KitId, Grupo = i.Grupo, FuncionarioId = update.Id });
                    }
                    else
                    {
                        var atv = item.TarefaAssociadas.Single(x => x.Id == i.Id);
                        atv.KitId = i.KitId;
                        atv.Grupo = i.Grupo;
                    }
                }

            if (update.TarefaAssociadasToDelete != null)
                foreach (var i in update.TarefaAssociadasToDelete)
                {
                    item.TarefaAssociadas.RemoveAll(x => x.Id == i.Id);
                }

            db.SaveChanges();
            return Ok(item);
        }


        [HttpGet("newcode")]
        public IActionResult GetCode()
        {
            return Ok(GenerateCode(db.Funcionarios.Select(x => x.Codigo)));
        }

        private static string GenerateCode(IEnumerable<string> codes)
        {
            int bestValue = 0;
            string bestCode = string.Empty;

            var rnd = new Random();
            foreach (var item in Enumerable.Range(1, 999).OrderBy(x => rnd.Next()))
            {
                var currentCode = item.ToString().PadLeft(3, '0');

                if (!codes.Any(x => x == currentCode))
                {
                    int value = CalculateValue(codes.Select(x => x), currentCode);

                    if (value == 200)
                        return currentCode;

                    if (bestValue < value)
                    {
                        bestValue = value;
                        bestCode = currentCode;
                    }
                }
                else
                {
                    continue;
                }
            }
            return bestCode;
        }

        private static int CalculateValue(IEnumerable<string> codes, string tmp)
        {
            var vinteequatro = new char[] { '2', '4' };
            return (tmp.Take(2).SequenceEqual(vinteequatro) || tmp.Skip(1).Take(2).SequenceEqual(vinteequatro) ? -5 : 0)
                   + (100 - codes.Count(x => x.Take(2).SequenceEqual(tmp.Take(2))))
                   + (100 - codes.Count(x => x.Skip(1).Take(2).SequenceEqual(tmp.Skip(1).Take(2))));
        }
    }
}
