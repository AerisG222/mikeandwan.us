import { State, Action, StateContext } from '@ngxs/store';
import { AuthSuccess, AuthFailed } from './auth.actions';
import { User } from 'oidc-client';

export interface AuthStateModel {
    user: User;
}

@State<AuthStateModel>({
    name: 'auth',
    defaults: {
        user: null
    }
})
export class UploadAppState {
    @Action(AuthSuccess)
    authSuccess(ctx: StateContext<AuthStateModel>) {

    }

    @Action(AuthFailed)
    loadServerFiles(ctx: StateContext<AuthStateModel>) {

    }
}
