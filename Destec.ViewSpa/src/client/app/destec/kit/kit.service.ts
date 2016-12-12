import { Injectable } from '@angular/core';
import { Response, Http, URLSearchParams } from '@angular/http';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { Observable } from 'rxjs/Rx';
import { Config } from '../../shared/config/env.config';
import { Kit } from './Kit.model';

@Injectable()
export class KitService {

    private url: string = Config.API + 'kit';
    private pedidoUrl: string = Config.API + 'pedido';

    constructor(private http: Http) { }

    getKits(filterText: string): Observable<Kit[]> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('filter', filterText);

        return this.http.get(this.url, { search: params })
            .map((res: Response) => res.json())
            .catch(this.handleError);
    }

    getKitById(id: string): Observable<Kit> {
        return this.http.get(this.url + '/' + id)
            .map(this.handleResult)
            .catch(this.handleError);
    }

    postKit(Kit: Kit) {
        return this.http.post(this.url, Kit)
            .map(this.handleResult)
            .catch(this.handleError);
    }

    putKit(Kit: Kit) {
        return this.http
            .put(this.url, JSON.stringify(Kit))
            .map(this.handleResult)
            .catch(this.handleError);
    }

    deleteKit(id: number) {
        return this.http.delete(this.url + '/' + id)
            .map(this.handleResult)
            .catch(this.handleError);
    }

    generateKit(kitId: number) {
        return this.http.get(this.url + '/gerar/' + kitId)
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
