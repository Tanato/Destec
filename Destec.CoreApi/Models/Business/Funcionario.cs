using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Destec.CoreApi.Models.Business
{
    public class Funcionario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Codigo { get; set; }

        public bool Inativo { get; set; }
        
        public List<TarefaAssociada> TarefaAssociadas { get; set; }

        public int? AtividadeAtualId { get; set; }
        public int? AtividadeAjudaId { get; set; }
    }

    public class TarefaAssociada
    {
        public int Id { get; set; }

        public int FuncionarioId { get; set; }
        public Funcionario Funcionario { get; set; }

        public int KitId { get; set; }
        public Kit Kit { get; set; }

        public int Grupo { get; set; }
    }
}
