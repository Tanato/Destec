import { Component, OnInit } from '@angular/core';
import { AtividadeService } from './atividade.service';
import { Atividade } from './atividade.model';
import { Funcionario } from '../funcionario/funcionario.model';
import { FuncionarioService } from '../funcionario/funcionario.service';

import { ToastsManager } from 'ng2-toastr/ng2-toastr';
import * as _ from 'lodash';

@Component({
    moduleId: module.id,
    selector: 'atividademaster-cmp',
    templateUrl: './atividade.master.component.html',
})
export class AtividadeMasterComponent implements OnInit {

    private data: Atividade[];
    private rows: Atividade[];

    private funcionarios: Funcionario[];

    private totalItems: number = 0;
    private currentPage: number = 1;
    private itemsPerPage: number = 50;

    private filterText: string = '';
    private editId: number;

    private finalizadas: boolean;
    private canceladas: boolean;

    constructor(private service: AtividadeService,
        private funcionarioService: FuncionarioService,
        private toastr: ToastsManager) { }

    ngOnInit() {
        this.filter();

        this.funcionarioService.getFuncionarios('')
            .subscribe(response => {
                this.funcionarios = response;
            });
    }

    filter() {
        var status = [0, 1, 2]
        if (this.finalizadas)
            status.push(3);
        if (this.canceladas)
            status.push(4);

        this.service.getAtividades(this.filterText, status)
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

    onDeallocate(item: Atividade) {
        this.service.deallocateAtividade(item.id)
            .subscribe((data: any) => {
                this.toastr.success('Atividades liberadas com sucesso!');
                this.filter();
            });;
    }

    onPageChange(page: any, data: Array<any> = this.data) {
        let start = (page.page - 1) * page.itemsPerPage;
        let end = page.itemsPerPage > -1 ? (start + page.itemsPerPage) : data.length;
        this.rows = data.slice(start, end);
    }

    refreshFuncionario(value: any) {
        _.forEach(this.rows, (item: Atividade) => {
            if (item.selected)
                item.funcionario = value;
            item.funcionarioId = value.id;
        });
    }

    updateFuncionario() {
        this.service.updateFuncionario(_.filter(this.rows, (x: Atividade) => x.selected))
            .subscribe((x: any) => {
                this.filter();
            });
    }

    editClick(id: number) {
        this.editId = id;
    }

    checkbox(value: Atividade) {
        value.selected = !value.selected;
        _.forEach(this.rows, (item: Atividade) => {
            if (item.kitPedidoId === value.kitPedidoId && item.tipoAtividade.grupo === value.tipoAtividade.grupo)
                item.selected = value.selected;
            else
                item.selected = false;
        });
    }
}
