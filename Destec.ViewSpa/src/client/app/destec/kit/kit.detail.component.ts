import { Component, OnInit } from '@angular/core';
import { Kit, TipoAtividade } from './kit.model';
import { KitService } from './kit.service';
import { Observable } from 'rxjs/Observable';
import { ActivatedRoute, Router } from '@angular/router';
import * as _ from 'lodash';

import { ToastsManager } from 'ng2-toastr/ng2-toastr';

@Component({
    moduleId: module.id,
    selector: 'kitdetail-cmp',
    templateUrl: './kit.detail.component.html'
})
export class KitDetailComponent implements OnInit {

    public model: Kit = new Kit;
    public tipoAtividade: TipoAtividade = new TipoAtividade;

    public formType: string = 'new';
    public blockEdit: boolean = true;

    public maskTimespan = [/\d/, /\d/, ':', /\d/, /\d/, ':', /\d/, /\d/];

    public range: number[] = _.range(1, 101);

    public id: Observable<string>;

    constructor(public service: KitService,
        private route: ActivatedRoute,
        private router: Router,
        private toastr: ToastsManager) { }

    ngOnInit() {
        this.model.tipoAtividades = [];

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

    save() {
        if (this.formType === 'new' && !this.model.id) {
            this.service.postKit(this.model)
                .subscribe((data: any) => {
                    this.model = data;
                    this.toastr.success('Kit criado com sucesso!');
                });
        } else {
            this.service.putKit(this.model)
                .subscribe((data: any) => {
                    this.model = data;
                    this.toastr.success('Kit salvo com sucesso!');
                });
        }
    }

    addAtividade() {
        // Se new, apenas insere, se edit apenas altera.
        if (this.formType === 'new' || !this.tipoAtividade.id) {
            this.tipoAtividade.kitId = this.model.id;
            this.model.tipoAtividades.push(this.tipoAtividade);
        } else if (this.tipoAtividade.id > 0) {
            let i = this.model.tipoAtividades.findIndex(x => x.id === this.tipoAtividade.id)
            this.model.tipoAtividades.splice(i, 1, this.tipoAtividade);
        }
        this.tipoAtividade = new TipoAtividade();
        this.tipoAtividade.tempoEstimado = null;
    }

    selectRow(item: TipoAtividade) {
        if (this.formType === 'edit') {
            this.tipoAtividade = _.cloneDeep(item);
        }
    }

    onDelete(item: TipoAtividade) {
        _.remove(this.model.tipoAtividades, (n: TipoAtividade) => n.nome === item.nome && n.ordem === item.ordem);
    }

    onRefresh(id: string) {
        this.service.getKitById(id)
            .subscribe((data: Kit) => {
                this.model = data;
            });
    }

    onCancel() {
        this.id.subscribe(id => {
            if (id) {
                let link = 'destec/kit/cadastro';
                this.router.navigateByUrl(link);
            } else {
                let link = 'destec/kit/cadastro';
                this.router.navigateByUrl(link);
            }
        });
    }

    // ToDo: Remover
    gerarPedido() {
        this.service.generateKit(this.model.id)
            .subscribe((data: any) => {
                this.toastr.success('Atividades geradas com sucesso!');
            });
    }
}
