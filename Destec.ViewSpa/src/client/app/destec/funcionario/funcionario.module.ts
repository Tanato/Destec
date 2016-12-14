import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

import { FuncionarioMasterComponent } from './funcionario.master.component';
import { FuncionarioDetailComponent } from './funcionario.detail.component';

import { FuncionarioService } from './funcionario.service';

import { PaginationModule, ModalModule } from 'ng2-bootstrap/ng2-bootstrap';
import { TextMaskModule } from 'angular2-text-mask';
import { ToastModule } from 'ng2-toastr/ng2-toastr';

import { SelectModule } from 'ng2-select/ng2-select';
import { Ng2AutoCompleteModule } from 'ng2-auto-complete';

let options: any = {
    animate: 'flyRight',
    positionClass: 'toast-bottom-right',
};

@NgModule({
    imports: [CommonModule, PaginationModule, RouterModule, TextMaskModule,
        ModalModule, ToastModule.forRoot(options), SelectModule],
    providers: [FuncionarioService],
    declarations: [FuncionarioMasterComponent],
    exports: [FuncionarioMasterComponent]
})
export class FuncionarioMasterModule { }

@NgModule({
    imports: [CommonModule, PaginationModule, RouterModule, TextMaskModule,
        ModalModule, ToastModule.forRoot(options), SelectModule, Ng2AutoCompleteModule],
    providers: [FuncionarioService],
    declarations: [FuncionarioDetailComponent],
    exports: [FuncionarioDetailComponent]
})
export class FuncionarioDetailModule { }
