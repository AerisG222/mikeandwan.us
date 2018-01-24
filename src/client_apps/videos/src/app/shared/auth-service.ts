import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { tap, shareReplay } from 'rxjs/operators';
import 'rxjs/add/observable/from';

import * as moment from 'moment';
import * as Oidc from 'oidc-client';

@Injectable()
export class AuthService {
    private readonly TOKEN_NAME = 'maw_videos_token';
    private _mgr: Oidc.UserManager;

    constructor() {
        // TODO: config and/or manager to come from di
        const config = {
            authority: 'https://localhost:5001',
            client_id: 'maw_videos',
            redirect_uri: 'https://localhost:5021/videos/signin-oidc',
            response_type: 'id_token token',
            scope: 'openid video role',
            loadUserInfo: true,
            post_logout_redirect_uri : 'https://localhost:5021/'
        };

        this._mgr = new Oidc.UserManager(config);
    }

    login() {
        this._mgr.signinRedirect();
    }

    completeLogin(): Observable<boolean> {
        return Observable.create(observer => {
            this._mgr
                .signinRedirectCallback()
                .then(user => {
                    this.setUser(user);

                    observer.next(true);
                    observer.complete();
                }, x => {
                    observer.next(false);
                    observer.complete();
                });
        });
    }

    logout() {
        localStorage.removeItem(this.TOKEN_NAME);

        this._mgr.signoutRedirect();
    }

    isLoggedIn() {
        return this.getToken() != null;
    }

    getToken() {
        const usr = this.getUser();

        if (usr == null || usr.expired) {
            return null;
        }

        return usr.access_token;
    }

    private getUser(): Oidc.User {
        const json = localStorage.getItem(this.TOKEN_NAME);

        if (json) {
            return <Oidc.User>JSON.parse(json);
        }

        return null;
    }

    private setUser(user: Oidc.User) {
        localStorage.setItem(this.TOKEN_NAME, JSON.stringify(user));
    }
}
