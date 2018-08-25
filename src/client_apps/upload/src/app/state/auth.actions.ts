import { User } from 'oidc-client';

export class CompleteSignin {
    static readonly type = '[Signin Handler] Complete Signin';
}

export class UpdateUser {
    static readonly type = '[Auth Service] Update User';

    constructor(public user: User) { }
}

export class ShowUsername {
    static readonly type = '[Auth Service] Show Username';

    constructor(public doShowUsername: boolean) { }
}
