using Destec.CoreApi.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Destec.CoreApi.Models.Business
{
    public class Pedido
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }

        //public int Ordem { get; set; } // ToDo, not used

        public DateTime? DataPedido { get; set; }
        public DateTime? Prazo { get; set; }

        public bool Cancelado { get; set; }

        public List<PedidoItem> Itens { get; set; }

        public StatusEnum Status { get; set; }
    }
}
