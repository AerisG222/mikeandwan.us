import { Injectable } from '@angular/core';
import { EnvironmentConfig } from '../models/environment-config';
import { UserManager, User } from 'oidc-client';
import { Store } from '@ngxs/store';
import { UpdateUser, ShowUsername } from '../state/auth.actions';

@Injectable()
export class AuthService {
    private _mgr: UserManager;
    private _user: User;

    constructor(private _store: Store,
                cfg: EnvironmentConfig) {
        const config = {
            automaticSilentRenew: true,
            silent_redirect_uri: `${cfg.wwwUrl}/account/spa-silent-signin`,
            authority: cfg.authUrl,
            client_id: 'maw_upload',
            redirect_uri: `${cfg.wwwUrl}/upload/signin-oidc`,
            response_type: 'id_token token',
            scope: 'openid maw_api role',
            loadUserInfo: true,
            post_logout_redirect_uri: `${cfg.wwwUrl}/`
        };

        this._mgr = new UserManager(config);

        this._mgr.events.addUserLoaded(user => {
            this.updateUser(user);
        });

        this._mgr.getUser().then(user => {
            this.updateUser(user);
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
            this.updateUser(user);
        });
    }

    attemptSilentSignin(): Promise<void> {
        return this._mgr.signinSilent().then(user => {
            this.updateUser(user);
        });
    }

    private updateUser(user: User) {
        this._user = user;

        if (user) {
            this._store.dispatch(new ShowUsername(user.profile.role.includes('admin')));
        }

        this._store.dispatch(new UpdateUser(user));
    }
}
