using System;

namespace Destec.CoreApi.Models.ViewModels
{
    public class AtividadeReportViewModel
    {
        public int Id { get; set; }
        public int KitPedidoId { get; set; }

        public int TipoAtividadeId { get; set; }
        public string TipoAtividade { get; set; }

        public int Ordem { get; set; }
        public int Grupo { get; set; }
        public string Kit { get; set; }

        public int PedidoId { get; set; }
        public string Pedido { get; set; }

        public int? FuncionarioId { get; set; }
        public string Funcionario { get; set; }
        
        public int? AjudanteId { get; set; }
        public string Ajudante { get; set; }

        public DateTime? DataInicio { get; set; }
        public DateTime? DataFinal { get; set; }

        public string TempoFormatted { get; set; }
        
        public TimeSpan? Intervalo { get; set; }
        public TimeSpan? Parada { get; set; }
        public TimeSpan? Ajuda { get; set; }

        public string IntervaloFormatted { get; set; }
        public string ParadaFormatted { get; set; }
        public string AjudaFormatted { get; set; }
        
        public string Status { get; set; }
    }
}
