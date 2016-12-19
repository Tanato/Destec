using System.ComponentModel;

namespace Destec.CoreApi.Shared.Enum
{
    public enum AtividadeStatusEnum
    {
        Criada,
        Alocada,
        [Description("Em Execução")]
        EmExecucao,
        Finalizada,
        Cancelada,
    }
}
