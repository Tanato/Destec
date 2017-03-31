using Destec.CoreApi.Models.Business;
using Destec.CoreApi.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Destec.CoreApi.Models.ViewModels
{
    public class AtividadeViewModel
    {
        public int Id { get; set; }
        public int KitPedidoId { get; set; }
        public string KitNumero { get; set; }

        public int TipoAtividadeId { get; set; }
        public TipoAtividade TipoAtividade { get; set; }

        public int PedidoItemId { get; set; }
        public PedidoItem PedidoItem { get; set; }

        public int? FuncionarioId { get; set; }
        public Funcionario Funcionario { get; set; }
        
        public int? AjudanteId { get; set; }
        public Funcionario Ajudante { get; set; }

        public DateTime? DataInicio { get; set; }
        public DateTime? DataFinal { get; set; }

        public DateTime? IntervaloFrom { get; set; }
        public TimeSpan? Intervalo { get; set; }

        public string IntervaloFormatted { get; set; }
        public string TempoFormatted { get; set; }

        public DateTime? ParadaFrom { get; set; }
        public TimeSpan? Parada { get; set; }
        
        public DateTime? AjudaFrom { get; set; }
        public TimeSpan? Ajuda { get; set; }

        public AtividadeStatusEnum Status { get; set; }
        public string StatusDescricao { get; set; }
        
        public double TempoCorrente { get; set; }
        public double IntervaloCorrente { get; set; }

        public bool InParada { get; set; }
        public bool InIntervalo { get; set; }
        public bool InAjuda { get; set; }
    }
}
