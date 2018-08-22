import { User } from 'oidc-client';

export class AuthSuccess {
    static readonly type = '[Signin Handler] Auth Success';

    constructor(public user: User) { }
}

export class AuthFailed {
    static readonly type = '[Signin Handler] Auth Failed';
}
