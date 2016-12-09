import { Injectable } from '@angular/core';
import { Response, Http } from '@angular/http';
import { Cookie } from 'ng2-cookies/ng2-cookies';

import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { Observable } from 'rxjs/Rx';

import { Config } from '../../shared/config/env.config';

@Injectable()
export class TopnavService {

    private url: string = Config.API + 'account';

    constructor(private http: Http) { }

    getLoggedUser() {
        return this.http.get(this.url + '/loggeduser')
            .map(this.handleResult)
            .catch(this.handleError);
    }

    logout() {
        return this.clearCookie(this.http.get(this.url + '/logout')
            .catch(this.handleError));
    }

    handleResult(res: Response) {
        let body = res.json();
        return body || {};
    }

    handleError(error: any) {
        console.error(error);
        return Observable.throw(error.json().Error || 'Server error');
    }

    clearCookie(observable: Observable<Response>): Observable<Response> {
        return observable.finally(() => Cookie.deleteAll());
    }
}

