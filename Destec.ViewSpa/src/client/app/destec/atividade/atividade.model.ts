import { Kit } from '../kit/kit.model';
import { PedidoItem } from '../pedido/pedido.model';
import { Funcionario } from '../funcionario/funcionario.model';

export class Atividade {
    id: number;
    kitPedidoId: string;
    kitNumero: string;
    pedidoItem: PedidoItem;
    pedidoItemId: number;
    funcionario: Funcionario;
    funcionarioId: number;
    tipoAtividade: TipoAtividade;
    tipoAtividadeId: number;
    dataInicio: any;
    dataFinal: any;
    intervalo: any;
    parada: any;
    status: number;
    statusDescricao: string;
    selected: boolean;

    inIntervalo: boolean;
    inParada: boolean;
    inAjuda: boolean;
    
    timer: any;
    timerSubs: any;
    tempoCorrente: number;
    tempoTimer: any;
    intervaloCorrente: any;
    intervaloTimer: any;
}

export class TipoAtividade {
    id: number;
    nome: string;
    kitId: number;
    kit: Kit;
    ordem: number;
    grupo: number;
    tempoEstimado: any;
    pontos: number;
}
