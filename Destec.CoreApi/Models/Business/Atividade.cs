using Destec.CoreApi.Shared.Enum;
using System;

namespace Destec.CoreApi.Models.Business
{
    public class Atividade
    {
        public int Id { get; set; }
        public int KitPedidoId { get; set; }

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
        
        public DateTime? ParadaFrom { get; set; }
        public TimeSpan? Parada { get; set; }

        public int? AjudanteId { get; set; }
        public Funcionario Ajudante { get; set; }

        public DateTime? AjudaFrom { get; set; }
        public TimeSpan? Ajuda { get; set; }

        public AtividadeStatusEnum Status { get; set; }
    }
}
