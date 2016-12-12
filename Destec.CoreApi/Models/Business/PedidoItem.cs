using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Destec.CoreApi.Models.Business
{
    public class PedidoItem
    {
        public int Id { get; set; }
        public string Observacao { get; set; }

        public int Quantidade { get; set; }

        public int KitId { get; set; }
        public Kit Kit { get; set; }

        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; }

        public bool Cancelado { get; set; }

        public List<Atividade> Atividades { get; set; }
    }
}
