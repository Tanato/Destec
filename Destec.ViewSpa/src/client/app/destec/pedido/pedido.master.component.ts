import { Component, OnInit } from '@angular/core';
import { PedidoService } from './pedido.service';
import { Pedido } from './pedido.model';

import { ToastsManager } from 'ng2-toastr/ng2-toastr';

@Component({
    moduleId: module.id,
    selector: 'pedidomaster-cmp',
    templateUrl: './pedido.master.component.html',
})
export class PedidoMasterComponent implements OnInit {

    private data: Pedido[];
    private rows: Pedido[];

    private totalItems: number = 0;
    private currentPage: number = 1;
    private itemsPerPage: number = 50;

    private filterText: string = '';
    private editId: number;

    constructor(private service: PedidoService,
                private toastr: ToastsManager) { }

    ngOnInit() {
        this.filter();
    }

    filter() {
        this.service.getPedidos(this.filterText)
            .subscribe(response => {
                this.data = response;
                this.totalItems = this.data.length;
                this.onPageChange({ page: this.currentPage, itemsPerPage: this.itemsPerPage });
            },
            error => {
                alert(error);
                console.log(error);
                this.toastr.warning('Erro ao efetuar operação. Tente novamente');
            });
    }

    onPageChange(page: any, data: Array<any> = this.data) {
        let start = (page.page - 1) * page.itemsPerPage;
        let end = page.itemsPerPage > -1 ? (start + page.itemsPerPage) : data.length;
        this.rows = data.slice(start, end);
    }

    editClick(id: number) {
        this.editId = id;
    }
}
