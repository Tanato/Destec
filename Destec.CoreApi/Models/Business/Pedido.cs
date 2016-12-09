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

        public DateTime? DataPedido { get; set; }
        public DateTime? Prazo { get; set; }

        public bool Cancelado { get; set; }

        public List<PedidoItem> Itens { get; set; }
    }
}
