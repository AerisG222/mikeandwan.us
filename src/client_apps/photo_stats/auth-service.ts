import * as Oidc from 'oidc-client';

export class AuthService {
    readonly TOKEN_NAME = 'maw_photo_stats_token';

    private _mgr: Oidc.UserManager;


    constructor() {
        const config = {
            authority: 'https://localhost:5001',
            client_id: 'maw_photo_stats',
            redirect_uri: 'https://localhost:5021/photos/stats/signin-oidc',
            response_type: 'id_token token',
            scope: 'openid maw_api role',
            loadUserInfo: true,
            post_logout_redirect_uri : 'https://localhost:5021/'
        };

        this._mgr = new Oidc.UserManager(config);
    }

    initSession() {
        if (!this.isLoggedIn()) {
            if(document.location.pathname.indexOf('signin-oidc') > 0) {
                this.completeLogin()
                    .then(x => {
                        window.location.href = '/photos/stats';
                    });
            } else {
                this.login();
            }
        }
    }

    login() {
        this._mgr.signinRedirect();
    }

    completeLogin(): Promise<boolean> {
        return this._mgr
                .signinRedirectCallback()
                .then(user => {
                    this.setUser(user);

                    return true;
                }, x => false);
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

        return usr === null ? null : usr.access_token;
    }

    private getUser(): Oidc.User {
        const json = localStorage.getItem(this.TOKEN_NAME);

        if (json) {
            const usr = <Oidc.User>JSON.parse(json);

            if (usr == null) {
                return null;
            }

            if (this.isExpired(usr)) {
                this.setUser(null);
                return null;
            }

            return usr;
        }

        return null;
    }

    // while the true Oidc.User has an expired property, we rehydrate this from JSON,
    // so do not actually have a true instance and expired is a property, not a field
    // https://github.com/IdentityModel/oidc-client-js/blob/dev/src/User.js
    private isExpired(user: Oidc.User) {
        const now = Date.now() / 1000;
        const expiresIn = user.expires_at - now;

        if (expiresIn !== undefined) {
            return expiresIn <= 0;
        }
    }

    private setUser(user: Oidc.User) {
        if (user == null) {
            localStorage.removeItem(this.TOKEN_NAME);
        } else {
            localStorage.setItem(this.TOKEN_NAME, JSON.stringify(user));
        }
    }
}
