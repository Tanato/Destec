import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { DropdownModule } from 'ng2-bootstrap/ng2-bootstrap';
import { ModalModule } from 'ng2-bootstrap/ng2-bootstrap';

import { KitMasterModule, KitDetailModule } from './kit/kit.module';
import { FuncionarioMasterModule, FuncionarioDetailModule } from './funcionario/funcionario.module';

import { DestecComponent } from './destec.component';

import { TopNavComponent } from '../shared/index';
import { SidebarComponent } from '../shared/index';

@NgModule({
    imports: [
        CommonModule,
        RouterModule,
        DropdownModule,
        ModalModule,
        KitMasterModule,
        KitDetailModule,
        FuncionarioMasterModule,
        FuncionarioDetailModule,
    ],
    declarations: [DestecComponent, TopNavComponent, SidebarComponent],
    exports: [DestecComponent, TopNavComponent, SidebarComponent]
})
export class DestecModule { }
