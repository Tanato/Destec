import { Component, OnInit } from '@angular/core';
import { KitService } from '../kit/kit.service';
import { Kit } from '../kit/kit.model';
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

    kits: Kit[];
    grupos: Observable<number[]> = Observable.of(_.range(1, 50));

    public id: Observable<string>;

    constructor(public service: FuncionarioService,
        private kitService: KitService,
        private route: ActivatedRoute,
        private router: Router,
        private toastr: ToastsManager) { }

    ngOnInit() {

        this.model.tarefaAssociadas = [];

        this.kitService.getKitSelect()
            .subscribe((data: any) => {
                this.kits = data;
            });

        this.id = this.route.params.map(params => params['id']);

        this.id.subscribe(id => {
            if (id) {
                this.formType = 'edit';
                this.blockEdit = true;
                this.onRefresh(id);
            } else {
                this.model.inativo = false;
                this.blockEdit = false;
            }
        });
    }

    updateGrupo(event: any) {
        this.tarefaAssociada.kitId = event;
        this.tarefaAssociada.kit = this.kits.find((x: Kit) => x.id === event);
        this.grupos = this.kitService.getGrupoSelect(event.toString());
    }

    save() {
        if (this.formType === 'new' && !this.model.id) {
            this.service.postFuncionario(this.model)
                .subscribe((data: any) => {
                    this.model = data;
                    this.tarefaAssociada = new TarefaAssociada();
                    this.toastr.success('Funcionario criado com sucesso!');
                    this.onCancel();
                });
        } else {
            this.service.putFuncionario(this.model)
                .subscribe((data: any) => {
                    this.model = data;
                    this.tarefaAssociada = new TarefaAssociada();
                    this.toastr.success('Funcionario salvo com sucesso!');
                    this.onCancel();
                });
        }
    }

    addAtividade() {
        // Se new, apenas insere, se edit apenas altera.
        if (this.formType === 'new') {
            this.tarefaAssociada.funcionarioId = this.model.id;
            this.model.tarefaAssociadas.push(this.tarefaAssociada);
        } else if (this.tarefaAssociada.id > 0) {
            let i = this.model.tarefaAssociadas.findIndex(x => x.id === this.tarefaAssociada.id)
            this.model.tarefaAssociadas.splice(i, 1, this.tarefaAssociada);
        } else {
            this.tarefaAssociada.funcionarioId = this.model.id;
            this.model.tarefaAssociadas.push(this.tarefaAssociada);
        }
        this.tarefaAssociada = new TarefaAssociada();
    }

    selectRow(item: TarefaAssociada) {
        if (this.formType === 'edit') {
            this.tarefaAssociada = _.cloneDeep(item);
        }
    }

    cancelChange() {
        this.tarefaAssociada = new TarefaAssociada();
    }

    onDelete(item: TarefaAssociada) {
        _.remove(this.model.tarefaAssociadas, (n: TarefaAssociada) => n.grupo === item.grupo
            && n.kitId === item.kitId
            && (item.id === 0 || n.id === item.id));

        if (this.formType === 'edit') {
            this.model.tarefaAssociadasToDelete.push(item);
        }
    }

    onRefresh(id: string) {
        this.service.getFuncionarioById(id)
            .subscribe((data: Funcionario) => {
                this.model = data;
                this.model.tarefaAssociadasToDelete = [];
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

    getCode() {
        this.service.getNewCode().subscribe((data: string) => {
            this.model.codigo = data;
        });
    }

    deallocateFuncionario() {
        this.service.deallocateFuncionario(this.model.id)
            .subscribe((data: any) => {
                this.toastr.success('Atividades geradas com sucesso!');
            });
    }
}
