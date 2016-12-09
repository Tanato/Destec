import { Injectable } from '@angular/core';
import { Http, Request, Response, RequestOptionsArgs, ConnectionBackend, RequestOptions, Headers } from '@angular/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import * as _ from 'lodash';

@Injectable()
export class AuthHttp extends Http {
    constructor(backend: ConnectionBackend,
        defaultOptions: RequestOptions,
        private router: Router) {
        super(backend, defaultOptions);
    }

    request(url: string | Request, options?: RequestOptionsArgs): Observable<Response> {
        return this.intercept(super.request(url, this.appendHeader(options)));
    }

    get(url: string, options?: RequestOptionsArgs): Observable<Response> {
        return this.intercept(super.get(url, this.appendHeader(options)));
    }

    post(url: string, body: any, options?: RequestOptionsArgs): Observable<Response> {
        return this.intercept(super.post(url, body, this.appendHeader(options)));
    }

    put(url: string, body: any, options?: RequestOptionsArgs): Observable<Response> {
        return this.intercept(super.put(url, body, this.appendHeader(options)));
    }

    delete(url: string, options?: RequestOptionsArgs): Observable<Response> {
        return this.intercept(super.delete(url, this.appendHeader(options)));
    }

    intercept(observable: Observable<Response>): Observable<Response> {
        return observable.catch((err, source) => {
            if (err.status === 401 && !_.endsWith(err.url, 'api/account/login')) {
                this.router.navigate(['/']);
                return Observable.empty();
            } else {
                return Observable.throw(err);
            }
        });
    }

    private appendHeader(options?: RequestOptionsArgs): RequestOptionsArgs {
        var headers = new Headers();
        headers.append('Content-Type', 'application/json');

        let mergedOptions: RequestOptionsArgs;
        if (!options) {
            mergedOptions = { headers: headers, withCredentials: true };
        } else {
            mergedOptions = options;
            mergedOptions.withCredentials = true;
        }
        return mergedOptions;
    }
}
