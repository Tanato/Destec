import { Component, OnInit } from '@angular/core';
import { Usuario } from './usuario.model';
import { UsuarioService } from './usuario.service';
import { Observable } from 'rxjs/Observable';
import { ActivatedRoute, Router } from '@angular/router';

import * as _ from 'lodash';

import { ToastsManager } from 'ng2-toastr/ng2-toastr';

@Component({
    moduleId: module.id,
    selector: 'usuariodetail-cmp',
    templateUrl: './usuario.detail.component.html'
})
export class UsuarioDetailComponent implements OnInit {

    public modelName = 'Usuário';

    public model: Usuario = new Usuario;
    public roles: any[];

    public formType: string = 'new';
    public blockEdit: boolean = true;

    public id: Observable<string>;

    constructor(public service: UsuarioService,
        private route: ActivatedRoute,
        private router: Router,
        private toastr: ToastsManager) {
    }

    selectedOptions(): string[] {
        return this.roles
            .filter(opt => opt.checked)
            .map(opt => opt.id);
    }

    enableEdit() {
        this.blockEdit = false;
    }

    ngOnInit() {

        this.service.getRolesSelect()
            .subscribe((data: any[]) => {
                data.forEach(element => {
                    element.checked = false;
                });
                this.roles = data;

                this.id = this.route.params.map(params => params['id']);

                this.id.subscribe(id => {
                    if (id) {
                        this.blockEdit = true;
                        this.formType = 'edit';
                        this.service.getUsuarioById(id)
                            .subscribe((user: Usuario) => {
                                user.birthDate = user.birthDate.slice(0, 10);
                                this.model = user;
                                _(this.roles).keyBy('id').at(user.roles).value()
                                    .map((x: any) => x.checked = true);
                            });
                    } else {
                        this.blockEdit = false;
                    }

                });
            });
    }

    onSubmit() {

        this.model.roles = this.selectedOptions();
        if (this.formType === 'new' && !this.model.id) {
            this.service.postUsuario(this.model)
                .subscribe(x => {
                    this.toastr.success(this.modelName + ' adicionado com sucesso!');
                    this.onCancel();
                });
        } else {
            this.service.putUsuario(this.model)
                .subscribe(x => {
                    this.toastr.success(this.modelName + ' atualizado com sucesso!');
                    this.onCancel();
                });
        }
    }

    onCancel() {
        let link = ['/destec/usuario'];
        this.router.navigate(link);
    }
}
