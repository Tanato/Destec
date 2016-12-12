using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Destec.CoreApi.Models.Business
{
    public class TipoAtividade
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public int KitId { get; set; }
        public Kit Kit { get; set; }

        public short Ordem { get; set; }
        public short Grupo { get; set; }

        public TimeSpan? TempoEstimado { get; set; }
        public decimal? Pontos { get; set; }

        public List<Atividade> Atividades { get; set; }
    }
}
