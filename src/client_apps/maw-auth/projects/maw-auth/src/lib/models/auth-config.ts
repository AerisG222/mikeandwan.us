export class AuthConfig {
    constructor(
        public authority: string,
        public client_id: string,
        public post_logout_redirect_uri: string,
        public redirect_uri: string,
        public silent_redirect_uri: string,
        public loadUserInfo = true,
        public automaticSilentRenew = true,
        public response_type = 'id_token token',
        public scope = 'openid maw_api role') {

    }
}
