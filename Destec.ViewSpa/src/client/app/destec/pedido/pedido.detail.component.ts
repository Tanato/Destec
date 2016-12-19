import { Component, OnInit } from '@angular/core';
import { KitService } from '../kit/kit.service';
import { Kit } from '../kit/kit.model';
import { Pedido, PedidoItem } from './pedido.model';
import { PedidoService } from './pedido.service';
import { Observable } from 'rxjs/Observable';
import { ActivatedRoute, Router } from '@angular/router';
import * as _ from 'lodash';

import { ToastsManager } from 'ng2-toastr/ng2-toastr';

@Component({
    moduleId: module.id,
    selector: 'pedidodetail-cmp',
    templateUrl: './pedido.detail.component.html'
})
export class PedidoDetailComponent implements OnInit {

    public model: Pedido = new Pedido;
    public pedidoItem: PedidoItem = new PedidoItem;

    kits: Kit[];
    public formType: string = 'new';
    public blockEdit: boolean = true;
    public id: Observable<string>;

    clientes = (startsWith: string): Observable<any[]> => {
        var result = this.service.getClienteSelect(startsWith);
        return result;
    }

    constructor(public service: PedidoService,
        private kitService: KitService,
        private route: ActivatedRoute,
        private router: Router,
        private toastr: ToastsManager) { }

    ngOnInit() {
        this.model.itens = [];

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
                this.blockEdit = false;
            }
        });
    }

    selectKit(event: any) {
        this.pedidoItem.kitId = event;
        this.pedidoItem.kit = this.kits.find((x: Kit) => x.id === event);
    }

    save() {
        if (this.formType === 'new' && !this.model.id) {
            this.service.postPedido(this.model)
                .subscribe((data: any) => {
                    this.model = data;
                    this.model.dataPedido = data.dataPedido ? data.dataPedido.slice(0, 10) : null;
                    this.model.prazo = data.prazo ? data.prazo.slice(0, 10) : null;
                    this.toastr.success('Pedido criado com sucesso!');
                });
        } else {
            this.service.putPedido(this.model)
                .subscribe((data: any) => {
                    this.model = data;
                    this.model.dataPedido = data.dataPedido ? data.dataPedido.slice(0, 10) : null;
                    this.model.prazo = data.prazo ? data.prazo.slice(0, 10) : null;
                    this.toastr.success('Pedido salvo com sucesso!');
                });
        }
    }

    addItem() {
        // Se new, apenas insere, se edit apenas altera.
        if (this.formType === 'new') {
            this.pedidoItem.pedidoId = this.model.id;
            this.model.itens.push(this.pedidoItem);
        } else if (this.pedidoItem.id > 0) {
            let i = this.model.itens.findIndex(x => x.id === this.pedidoItem.id)
            this.model.itens.splice(i, 1, this.pedidoItem);
        } else {
            this.pedidoItem.pedidoId = this.model.id;
            this.model.itens.push(this.pedidoItem);
        }
        this.pedidoItem = new PedidoItem();
    }

    selectRow(item: PedidoItem) {
        if (this.formType === 'edit') {
            this.pedidoItem = _.cloneDeep(item);
        }
    }

    onDelete(item: PedidoItem) {
        _.remove(this.model.itens, (n: PedidoItem) => n.kitId === item.kitId);
    }

    onRefresh(id: string) {
        this.service.getPedidoById(id)
            .subscribe((data: Pedido) => {
                this.model = data;
                this.model.dataPedido = data.dataPedido ? data.dataPedido.slice(0, 10) : null;
                this.model.prazo = data.prazo ? data.prazo.slice(0, 10) : null;
            });
    }

    onCancel() {
        this.id.subscribe(id => {
            if (id) {
                let link = 'destec/pedido/cadastro';
                this.router.navigateByUrl(link);
            } else {
                let link = 'destec/pedido/cadastro';
                this.router.navigateByUrl(link);
            }
        });
    }

    cancelChange() {
        this.pedidoItem = new PedidoItem();
    }

    // ToDo: Remover
    gerarPedido() {
        this.service.generatePedido(this.model.id)
            .subscribe((data: any) => {
                this.toastr.success('Atividades geradas com sucesso!');
            });
    }
}
