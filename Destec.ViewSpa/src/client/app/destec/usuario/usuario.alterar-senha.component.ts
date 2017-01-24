import { Component } from '@angular/core';
import { AlterarSenha } from './usuario.model';
import { UsuarioService } from './usuario.service';
import { ActivatedRoute, Router } from '@angular/router';

import { ToastsManager } from 'ng2-toastr/ng2-toastr';

@Component({
    moduleId: module.id,
    selector: 'usuarioalterarsenha-cmp',
    templateUrl: './usuario.alterar-senha.component.html'
})
export class UsuarioAlterarSenhaComponent {

    public modelName = 'Usuário';

    public model: AlterarSenha = new AlterarSenha;

    constructor(public service: UsuarioService,
        private route: ActivatedRoute,
        private router: Router,
        private toastr: ToastsManager) {
    }

    onSubmit() {
        this.service.postAlterarSenha(this.model)
            .subscribe(x => {
                this.toastr.success('Senha alterada com sucesso!');
                this.onCancel();
            });
    }

    onCancel() {
        let link = ['/destec/home'];
        this.router.navigate(link);
    }
}
