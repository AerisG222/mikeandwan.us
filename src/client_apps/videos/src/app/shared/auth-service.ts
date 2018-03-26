import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserManager, UserManagerSettings, User } from 'oidc-client';
import { Observable } from 'rxjs/Observable';
import { tap, shareReplay } from 'rxjs/operators';
import 'rxjs/add/observable/from';


@Injectable()
export class AuthService {
    private readonly TOKEN_NAME = 'maw_videos_token';
    private _mgr: UserManager;
    private _user: User;

    constructor() {
        // TODO: config and/or manager to come from di
        const config = {
            authority: 'https://localhost:5001',
            client_id: 'maw_videos',
            redirect_uri: 'https://localhost:5021/videos/signin-oidc',
            response_type: 'id_token token',
            scope: 'openid maw_api role',
            loadUserInfo: true,
            post_logout_redirect_uri: 'https://localhost:5021/'
        };

        this._mgr = new UserManager(config);

        this._mgr.getUser().then(user => {
            this._user = user;
        });
    }

    isLoggedIn(): boolean {
        return this._user != null && !this._user.expired;
    }

    getClaims(): any {
        return this._user.profile;
    }

    getAuthorizationHeaderValue(): string {
        return `${this._user.token_type} ${this._user.access_token}`;
    }

    startAuthentication(): Promise<void> {
        return this._mgr.signinRedirect();
    }

    completeAuthentication(): Promise<void> {
        return this._mgr.signinRedirectCallback().then(user => {
            this._user = user;
        });
    }
}
