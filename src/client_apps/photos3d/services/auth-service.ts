import { Config } from '../config';
import { UserManager, UserManagerSettings, User } from 'oidc-client';

export class AuthService {
    private _mgr: UserManager;
    private _user: User;

    constructor(cfg: Config) {
        const config = {
            automaticSilentRenew: true,
            silent_redirect_uri: `${cfg.wwwUrl}/account/spa-silent-signin`,
            authority: cfg.authUrl,
            client_id: 'maw_photos_3d',
            redirect_uri: `${cfg.wwwUrl}/photos/3d/signin-oidc`,
            response_type: 'id_token token',
            scope: 'openid maw_api role',
            loadUserInfo: true,
            post_logout_redirect_uri : `${cfg.wwwUrl}/`
        };

        this._mgr = new UserManager(config);
    }

    async initSessionAsync() {
        this._user = await this._mgr.getUser();

        if (!this.isLoggedIn()) {
            if (document.location.pathname.indexOf('signin-oidc') > 0) {
                await this.completeAuthenticationAsync();
                window.location.href = '/photos/3d';
            } else {
                this.startAuthenticationAsync();
            }
        }
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

    async startAuthenticationAsync(): Promise<void> {
        await this._mgr.signinRedirect();
    }

    async completeAuthenticationAsync(): Promise<void> {
        this._user = await this._mgr.signinRedirectCallback();
    }

    attemptSilentSignin(): Promise<void> {
        return this._mgr.signinSilent().then(user => {
            this._user = user;
        });
    }
}
