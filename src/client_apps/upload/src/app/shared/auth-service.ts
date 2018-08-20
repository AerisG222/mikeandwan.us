import { Injectable } from '@angular/core';
import { EnvironmentConfig } from './environment-config';
import { UserManager, UserManagerSettings, User } from 'oidc-client';

@Injectable()
export class AuthService {
    private _mgr: UserManager;
    private _user: User;

    constructor(cfg: EnvironmentConfig) {
        const config = {
            automaticSilentRenew: true,
            silent_redirect_uri: `${cfg.wwwUrl}/account/spa-silent-signin`,
            authority: cfg.authUrl,
            client_id: 'maw_videos',
            redirect_uri: `${cfg.wwwUrl}/videos/signin-oidc`,
            response_type: 'id_token token',
            scope: 'openid maw_api role',
            loadUserInfo: true,
            post_logout_redirect_uri: `${cfg.wwwUrl}/`
        };

        this._mgr = new UserManager(config);

        this._mgr.events.addUserLoaded(user => {
            this._user = user;
        });

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

    attemptSilentSignin(): Promise<void> {
        return this._mgr.signinSilent().then(user => {
            this._user = user;
        });
    }
}