using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Destec.CoreApi.Models.Business
{
    public class PedidoItem
    {
        public int Id { get; set; }

        public int KitId { get; set; }
        public Kit Kit { get; set; }

        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; }

        public string Observacao { get; set; }
        public bool Cancelado { get; set; }

        // Se usa quantidade, pode setar o número inicial dos kits gerado
        public int? Quantidade { get; set; }
        public int? NumeroInicial { get; set; }

        // Se usa AndarInicial, utiliza quantidade de apartamentos por andar.
        public int? AndarInicial { get; set; }
        public int? QuantidadeAndar { get; set; }
        public int? QuantidadeApartamentosAndar { get; set; }
        public string Bloco { get; set; }

        public List<Atividade> Atividades { get; set; }
    }
}
