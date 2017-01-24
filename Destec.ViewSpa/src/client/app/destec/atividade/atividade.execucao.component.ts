import { Component, OnInit, OnDestroy } from '@angular/core';
import { AtividadeService } from './atividade.service';
import { Atividade } from './atividade.model';
import { Observable } from 'rxjs/Rx';

import { ToastsManager } from 'ng2-toastr/ng2-toastr';
import * as _ from 'lodash';

@Component({
    moduleId: module.id,
    selector: 'atividadeexecucao-cmp',
    templateUrl: './atividade.execucao.component.html',
})
export class AtividadeExecucaoComponent implements OnInit, OnDestroy {

    private data: Atividade[];
    private refreshTimer: Observable<number> = Observable.timer(0, 30000);
    private refreshTimerSubs: any;

    constructor(private service: AtividadeService,
        private toastr: ToastsManager) { }

    ngOnInit() {
        this.refreshTimerSubs = this.refreshTimer.subscribe(() => this.filter());
    }

    ngOnDestroy() {
        this.refreshTimerSubs.unsubscribe();
         _.each(this.data, x => {
            x.timerSubs.unsubscribe();
        });
    }

    filter() {
         _.each(this.data, x => {
            x.timerSubs.unsubscribe();
        });
        this.service.getAtividadesExecucao()
            .subscribe(response => {
                this.data = response;

                _.each(this.data, x => {
                    x.timer = Observable.timer(0, 1000);
                    x.timerSubs = x.timer.subscribe((t: number) => {
                        if (!x.inParada) {
                            x.tempoCorrente = x.tempoCorrente + (x.inIntervalo ? 0 : 1000);
                            x.intervaloCorrente = x.intervaloCorrente + (x.inIntervalo ? 1000 : 0);
                        }
                        x.tempoTimer = this.secondsToHms(x.tempoCorrente / 1000);
                        x.intervaloTimer = this.secondsToHms(x.intervaloCorrente / 1000);
                    });
                });
            },
            error => {
                alert(error);
                console.log(error);
                this.toastr.warning('Erro ao efetuar operação. Tente novamente');
            });
    }

    tempoEstourado(item: Atividade) {
        return item.tempoCorrente > this.hmsToSecondsOnly(item.tipoAtividade.tempoEstimado) * 1000;
    }

    secondsToHms(d: number) {
        var h = Math.floor(d / 3600);
        var m = Math.floor(d % 3600 / 60);
        var s = Math.floor(d % 3600 % 60);

        return ((h > 0 ? h + ':' + (m < 10 ? '0' : '') : '') + m + ':' + (s < 10 ? '0' : '') + s);
    }

    hmsToSecondsOnly(time: string) {
        var p = time.split(':'),
            s = 0, m = 1;

        while (p.length > 0) {
            s += m * parseInt(p.pop(), 10);
            m *= 60;
        }

        return s;
    }
}
