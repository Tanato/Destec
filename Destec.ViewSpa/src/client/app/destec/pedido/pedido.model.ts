import { Kit } from '../kit/kit.model';

export class Pedido {
    id: number;
    codigo: string;
    descicao: string;
    dataPedido: any;
    prazo: any;
    cancelado: boolean;
    itens: PedidoItem[];
    status: number;
}

export class PedidoItem {
    id: number;
    observacao: string;
    pedidoId: number;
    pedido: Pedido;
    kitId: number;
    kit: Kit;

    quantidade: number;
    numeroInicial: number;
    
    andarInicial: number;
    quantidadeAndar: number;
    quantidadeApartamentosAndar: number;
    bloco: string;

    cancelado: boolean;
}