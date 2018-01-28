import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { tap, shareReplay } from 'rxjs/operators';
import 'rxjs/add/observable/from';

import * as Oidc from 'oidc-client';

export class AuthService {
    private _mgr: Oidc.UserManager;

    constructor() {
        const config = {
            authority: 'https://localhost:5001',
            client_id: 'maw_photos',
            redirect_uri: 'https://localhost:5021/photos/signin-oidc',
            response_type: 'id_token token',
            scope: 'openid maw_api role',
            loadUserInfo: true,
            post_logout_redirect_uri : 'https://localhost:5021/'
        };
    
        this._mgr = new Oidc.UserManager(config);
    }
}
