using Destec.CoreApi.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Destec.CoreApi.Models.ViewModels
{
    public class FuncionarioViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Codigo { get; set; }

        public bool Inativo { get; set; }

        public List<TarefaAssociada> TarefaAssociadas { get; set; }
        public List<TarefaAssociada> TarefaAssociadasToDelete { get; set; }

        public int? AtividadeAtualId { get; set; }

    }
}
