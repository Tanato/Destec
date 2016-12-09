import { Injectable } from '@angular/core';
import { Response, Http } from '@angular/http';

import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { Observable } from 'rxjs/Rx';

import { Config } from '../shared/config/env.config';
import { ILogin } from '../shared/interfaces';

@Injectable()
export class LoginService {

    private url: string = Config.API + 'account';

    constructor(private http: Http) { }

    userLogin(body: ILogin): Observable<Response> {
        return this.http.post(this.url + '/login', body)
            .catch(this.handleError);
    }

    handleError(error: any) {
        console.error(error);
        return Observable.throw(error.json().Error || 'Server error');
    }
}

