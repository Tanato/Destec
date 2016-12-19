import { Component, OnInit } from '@angular/core';
import { AtividadeService } from './atividade.service';
import { Atividade } from './atividade.model';

import { ToastsManager } from 'ng2-toastr/ng2-toastr';

@Component({
    moduleId: module.id,
    selector: 'atividademaster-cmp',
    templateUrl: './atividade.master.component.html',
})
export class AtividadeMasterComponent implements OnInit {

    private data: Atividade[];
    private rows: Atividade[];

    private totalItems: number = 0;
    private currentPage: number = 1;
    private itemsPerPage: number = 50;

    private filterText: string = '';
    private editId: number;

    constructor(private service: AtividadeService,
                private toastr: ToastsManager) { }

    ngOnInit() {
        this.filter();
    }

    filter() {
        this.service.getAtividades(this.filterText)
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
