export class Usuario {
    id: number;
    userName: string;
    nome: string;
    birthDate: any;
    email: string;
    roles: string[];
    perfis: string;
    inativo: boolean;
}

export class AlterarSenha {
    currentPassword: string;
    newPassword: string;
    confirmPassword: string;
}
