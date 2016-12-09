export class Kit {
    id: number;
    nome: string;
    descicao: string;
    inativo: boolean;
    tipoAtividades: TipoAtividade[];
}

export class TipoAtividade {
    id: number;
    kitId: number;
    nome: string;
    ordem: number;
    grupo: number;
    tempoEstimado: any;
    pontos: number;
}