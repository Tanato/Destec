import { Injectable } from '@angular/core';
import { Response, Http, URLSearchParams } from '@angular/http';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { Observable } from 'rxjs/Rx';
import { Config } from '../../shared/config/env.config';
import { Funcionario } from './Funcionario.model';

@Injectable()
export class FuncionarioService {

    private url: string = Config.API + 'funcionario';
    private urlAtividade: string = Config.API + 'atividade';

    constructor(private http: Http) { }

    getFuncionarios(filterText: string): Observable<Funcionario[]> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('filter', filterText);

        return this.http.get(this.url, { search: params })
            .map((res: Response) => res.json())
            .catch(this.handleError);
    }

    getFuncionarioById(id: string): Observable<Funcionario> {
        return this.http.get(this.url + '/' + id)
            .map(this.handleResult)
            .catch(this.handleError);
    }

    postFuncionario(Funcionario: Funcionario) {
        return this.http.post(this.url, Funcionario)
            .map(this.handleResult)
            .catch(this.handleError);
    }

    putFuncionario(Funcionario: Funcionario) {
        return this.http
            .put(this.url, JSON.stringify(Funcionario))
            .map(this.handleResult)
            .catch(this.handleError);
    }

    deallocateFuncionario(funcionarioId: number) {
        return this.http.get(this.urlAtividade + '/deallocate/' + funcionarioId)
            .map(this.handleResult)
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
