using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Destec.CoreApi.Models.Business
{
    public class Kit
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }

        //public int? Versao { get; set; }

        public bool Inativo { get; set; }

        public string ExternalCode { get; set; }

        public List<TipoAtividade> TipoAtividades { get; set; }
    }
}
