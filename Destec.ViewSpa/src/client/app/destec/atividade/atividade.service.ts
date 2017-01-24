import { Injectable } from '@angular/core';
import { Response, Http, URLSearchParams } from '@angular/http';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { Observable } from 'rxjs/Rx';
import { Config } from '../../shared/config/env.config';
import { Atividade } from './Atividade.model';

@Injectable()
export class AtividadeService {

    private url: string = Config.API + 'atividade';

    constructor(private http: Http) { }

    getAtividades(filterText: string, status: number[]): Observable<Atividade[]> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('filter', filterText);
        status.forEach(element => {
            params.append('status', element.toString());
        });

        return this.http.get(this.url, { search: params })
            .map((res: Response) => res.json())
            .catch(this.handleError);
    }

    getAtividadesExecucao() {
        return this.http.get(this.url + '/execucao')
            .map((res: Response) => res.json())
            .catch(this.handleError);
    }

    getAtividadeById(id: string): Observable<Atividade> {
        return this.http.get(this.url + '/' + id)
            .map(this.handleResult)
            .catch(this.handleError);
    }

    postAtividade(atividade: Atividade) {
        return this.http.post(this.url, atividade)
            .map(this.handleResult)
            .catch(this.handleError);
    }

    putAtividade(atividade: Atividade) {
        return this.http
            .put(this.url, JSON.stringify(atividade))
            .map(this.handleResult)
            .catch(this.handleError);
    }

    deleteAtividade(id: number) {
        return this.http.delete(this.url + '/' + id)
            .map(this.handleResult)
            .catch(this.handleError);
    }

    generateAtividade(atividadeId: number) {
        return this.http.get(this.url + '/gerar/' + atividadeId)
            .map(this.handleResult)
            .catch(this.handleError);
    }

    updateFuncionario(atividades: Atividade[]) {
        return this.http.post(this.url + '/manualallocate', atividades);
    }

    deallocateAtividade(atividadeId: number) {
        return this.http.get(this.url + '/deallocateactivity/' + atividadeId)
            .catch(this.handleError);
    }

    private handleResult(res: Response) {
        let body = res.json();
        return body || {};
    }

    private handleError(error: any) {
        console.error(error);
        return Observable.throw(error.json().Error || 'Server error');
    }
}
