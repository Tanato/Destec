import { Injectable } from '@angular/core';
import { Response, Http, URLSearchParams } from '@angular/http';

import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { Observable } from 'rxjs/Rx';

import { Config } from '../../shared/config/env.config';

import { Usuario, AlterarSenha } from './usuario.model';

@Injectable()
export class UsuarioService {

    private url: string = Config.API + 'user';
    private urlAccount: string = Config.API + 'account';
    private urlRole: string = Config.API + 'role';

    constructor(public http: Http) {
        console.info('AppSvc created' + this.http);
    }

    postUsuario(usuario: Usuario) {
        return this.http.post(this.urlAccount + '/register', usuario)
            .catch(this.handleError);
    }

    putUsuario(usuario: Usuario) {
        return this.http.put(this.urlAccount + '/register', usuario)
            .catch(this.handleError);
    }

    getUsuarios(filterText: string): Observable<Usuario[]> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('filter', filterText);

        return this.http.get(this.url, { search: params })
            .map((res: Response) => res.json())
            .catch(this.handleError);
    }

    getUsuarioById(id: string): Observable<Usuario> {
        return this.http.get(this.url + '/' + id)
            .map(this.handleResult)
            .catch(this.handleError);
    }

    getRolesSelect() {
        return this.http.get(this.urlRole)
            .map(this.handleResult)
            .catch(this.handleError);
    }

    deleteUsuario(id: number) {
        return this.http.delete(this.url + '/' + id)
            .map(this.handleResult)
            .catch(this.handleError);
    }

    postAlterarSenha(model: AlterarSenha) {
        return this.http.post(this.urlAccount + '/changepassword', model)
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

