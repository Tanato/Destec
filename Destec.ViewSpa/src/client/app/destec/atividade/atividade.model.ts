import { Kit } from '../kit/kit.model';
import { Pedido, PedidoItem } from '../pedido/pedido.model';
import { Funcionario } from '../funcionario/funcionario.model';

export class Atividade {
    id: number;
    kitPedidoId: string;
    pedidoItem: PedidoItem;
    funcionario: Funcionario;
    tipoAtividade: TipoAtividade;
    dataInicio: any;
    dataFinal: any;
    intervalo: any;
    parada: any;
    status: number;
    statusDescricao: string;
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