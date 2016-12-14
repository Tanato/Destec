import { Component, OnInit } from '@angular/core';
import { KitService } from '../kit/kit.service';
import { Funcionario, TarefaAssociada } from './funcionario.model';
import { FuncionarioService } from './funcionario.service';
import { Observable } from 'rxjs/Observable';
import { ActivatedRoute, Router } from '@angular/router';
import * as _ from 'lodash';

import { ToastsManager } from 'ng2-toastr/ng2-toastr';

@Component({
    moduleId: module.id,
    selector: 'funcionariodetail-cmp',
    templateUrl: './funcionario.detail.component.html'
})
export class FuncionarioDetailComponent implements OnInit {

    public model: Funcionario = new Funcionario;
    public tarefaAssociada: TarefaAssociada = new TarefaAssociada;

    public formType: string = 'new';
    public blockEdit: boolean = true;

    kits: Observable<any[]>;
    grupo: Observable<any[]>;

    public id: Observable<string>;

    constructor(public service: FuncionarioService,
        private kitService: KitService,
        private route: ActivatedRoute,
        private router: Router,
        private toastr: ToastsManager) { }

    ngOnInit() {
        this.kits = this.kitService.getKitSelect();

        this.model.tarefasAssociadas = [];

        this.id = this.route.params.map(params => params['id']);

        this.id.subscribe(id => {
            if (id) {
                this.formType = 'edit';
                this.blockEdit = true;
                this.onRefresh(id);
            } else {
                this.blockEdit = false;
            }
        });
    }

    updateGrupo(){
        this.grupo = this.kitService.getGrupoSelect(this.tarefaAssociada.kitId.toString());
    }

    save() {
        if (this.formType === 'new' && !this.model.id) {
            this.service.postFuncionario(this.model)
                .subscribe((data: any) => {
                    this.model = data;
                    this.toastr.success('Funcionario criado com sucesso!');
                });
        } else {
            this.service.putFuncionario(this.model)
                .subscribe((data: any) => {
                    this.model = data;
                    this.toastr.success('Funcionario salvo com sucesso!');
                });
        }
    }

    addAtividade() {
        // Se new, apenas insere, se edit apenas altera.
        if (this.formType === 'new') {
            this.tarefaAssociada.funcionarioId = this.model.id;
            this.model.tarefasAssociadas.push(this.tarefaAssociada);
        } else if (this.tarefaAssociada.id > 0) {
            let i = this.model.tarefasAssociadas.findIndex(x => x.id === this.tarefaAssociada.id)
            this.model.tarefasAssociadas.splice(i, 1, this.tarefaAssociada);
        } else {
            this.toastr.warning('Selecione um item para editar!');
        }
        this.tarefaAssociada = new TarefaAssociada();
    }

    selectRow(item: TarefaAssociada) {
        if (this.formType === 'edit') {
            this.tarefaAssociada = item;
        }
    }

    onDelete(item: TarefaAssociada) {
        this.model.tarefasAssociadas =
            _.remove(this.model.tarefasAssociadas, (n: TarefaAssociada) => n.grupo === item.grupo && n.kitId === item.kitId);
    }

    onRefresh(id: string) {
        this.service.getFuncionarioById(id)
            .subscribe((data: Funcionario) => {
                this.model = data;
            });
    }

    onCancel() {
        this.id.subscribe(id => {
            if (id) {
                let link = 'destec/funcionario/cadastro';
                this.router.navigateByUrl(link);
            } else {
                let link = 'destec/funcionario/cadastro';
                this.router.navigateByUrl(link);
            }
        });
    }

    // ToDo: Remover
    deallocateFuncionario() {
        this.service.deallocateFuncionario(this.model.id)
            .subscribe((data: any) => {
                this.toastr.success('Atividades geradas com sucesso!');
            });
    }
}
