using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Destec.CoreApi.Models.Business
{
    public class Atividade
    {
        public int Id { get; set; }
        public int GrupoPedidoId { get; set; }

        public int TipoAtividadeId { get; set; }
        public TipoAtividade TipoAtividade { get; set; }

        public int PedidoItemId { get; set; }
        public PedidoItem PedidoItem { get; set; }

        public int? FuncionarioId { get; set; }
        public Funcionario Funcionario { get; set; }
        
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFinal { get; set; }

        public DateTime? IntervaloFrom { get; set; }
        public TimeSpan? Intervalo { get; set; }
    }
}
