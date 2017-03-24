import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

import { KitMasterComponent } from './kit.master.component';
import { KitDetailComponent } from './kit.detail.component';

import { KitService } from './kit.service';

import { PaginationModule, ModalModule } from 'ng2-bootstrap/ng2-bootstrap';
import { TextMaskModule } from 'angular2-text-mask';
import { ToastModule } from 'ng2-toastr/ng2-toastr';

import { SelectModule } from 'ng2-select/ng2-select';
import { Ng2AutoCompleteModule } from 'ng2-auto-complete';
import { BusyModule, BusyConfig } from 'angular2-busy';


let options: any = {
    animate: 'flyRight',
    positionClass: 'toast-bottom-right',
};

var busyConfig = new BusyConfig({
	message: 'Gerando Pedido...',
});

@NgModule({
    imports: [CommonModule, PaginationModule, RouterModule, TextMaskModule,
        ModalModule, ToastModule.forRoot(options), SelectModule],
    providers: [KitService],
    declarations: [KitMasterComponent],
    exports: [KitMasterComponent]
})
export class KitMasterModule { }

@NgModule({
    imports: [CommonModule, PaginationModule, RouterModule, TextMaskModule,
        ModalModule, ToastModule.forRoot(options), SelectModule, Ng2AutoCompleteModule, BusyModule.forRoot(busyConfig)],
    providers: [KitService],
    declarations: [KitDetailComponent],
    exports: [KitDetailComponent]
})
export class KitDetailModule { }
