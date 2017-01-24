import { Injectable } from '@angular/core';
import { Response, Http, URLSearchParams } from '@angular/http';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { Observable } from 'rxjs/Rx';
import { Config } from '../../shared/config/env.config';
import { Pedido } from './Pedido.model';

@Injectable()
export class PedidoService {

    private url: string = Config.API + 'pedido';

    constructor(private http: Http) { }

    getPedidos(filterText: string): Observable<Pedido[]> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('filter', filterText);

        return this.http.get(this.url, { search: params })
            .map((res: Response) => res.json())
            .catch(this.handleError);
    }

    getPedidoById(id: string): Observable<Pedido> {
        return this.http.get(this.url + '/' + id)
            .map(this.handleResult)
            .catch(this.handleError);
    }

    postPedido(Pedido: Pedido) {
        return this.http.post(this.url, Pedido)
            .map(this.handleResult)
            .catch(this.handleError);
    }

    putPedido(Pedido: Pedido) {
        return this.http
            .put(this.url, JSON.stringify(Pedido))
            .map(this.handleResult)
            .catch(this.handleError);
    }

    deletePedido(id: number) {
        return this.http.delete(this.url + '/' + id)
            .map(this.handleResult)
            .catch(this.handleError);
    }

    generatePedido(pedidoId: number) {
        return this.http.get(this.url + '/gerar/' + pedidoId)
            .map(this.handleResult)
            .catch(this.handleError);
    }

    cancelPedido(pedidoId: number) {
        return this.http.get(this.url + '/cancelar/' + pedidoId)
            .map(this.handleResult)
            .catch(this.handleError);
    }

    getPedidoSelect(): Observable<string[]> {
        return this.http.get(this.url + '/pedidoselect')
            .map(this.handleResult)
            .catch(this.handleError);
    }

    getGrupoSelect(filterText: string = ''): Observable<number[]> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('filter', filterText);

        return this.http.get(this.url + '/gruposelect', { search: params })
            .map(this.handleResult)
            .catch(this.handleError);
    }

    getClienteSelect(filterText: string = ''): Observable<number[]> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('filter', filterText);

        return this.http.get(this.url + '/clientes', { search: params })
            .map(this.handleResult)
            .catch(this.handleError);
    }

    getPedidoStatusDescription(status: number) {

        switch (status) {
            case 0:
                return 'Criado';
            case 1:
                return 'Gerando';
            case 2:
                return 'Gerado';
            case 3:
                return 'Cancelado';
            case 4:
                return 'Finalizado';
            default:
                return '';
        }
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
