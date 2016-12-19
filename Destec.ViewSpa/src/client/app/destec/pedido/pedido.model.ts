import { Kit } from '../kit/kit.model';

export class Pedido {
    id: number;
    codigo: string;
    descicao: string;
    dataPedido: any;
    prazo: any;
    cancelado: boolean;
    itens: PedidoItem[];
}

export class PedidoItem {
    id: number;
    observacao: string;
    quantidade: number;
    pedidoId: number;
    pedido: Pedido;
    kitId: number;
    kit: Kit;
    cancelado: boolean;
}