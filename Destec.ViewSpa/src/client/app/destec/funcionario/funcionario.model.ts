import { Kit } from '../kit/kit.model';

export class Funcionario {
    id: number;
    nome: string;
    codigo: string;
    descicao: string;
    inativo: boolean;
    tarefaAssociadas: TarefaAssociada[];
    tarefaAssociadasToDelete: TarefaAssociada[];
}

export class TarefaAssociada {
    id: number;
    funcionarioId: number;
    funcionario: Funcionario;
    kitId: number;
    kit: Kit;
    grupo: number;
}