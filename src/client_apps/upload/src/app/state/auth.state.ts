import { State, Action, StateContext, Selector } from '@ngxs/store';
import { User } from 'oidc-client';
import { CompleteSignin, UpdateUser, ShowUsername } from './auth.actions';
import { AuthService } from 'maw-auth';
import { AuthStoreManagerService } from '../services/auth-store-manager.service';

export interface AuthStateModel {
    user: User;
    showUsername: boolean;
}

@State<AuthStateModel>({
    name: 'auth',
    defaults: {
        user: null,
        showUsername: false
    }
})
export class AuthState {
    constructor(private _authService: AuthService,
                private _authStoreManagerService: AuthStoreManagerService) {
        console.log('creating auth state');

        // TODO: this is a little lame.  we need to pull in AuthStoreManagerService
        // so that DI creates an instance, which then subscribes to the auth service
        // events.  consider refactoring to make this a more natural integration
        console.log(this._authStoreManagerService);
    }

    @Selector()
    static getUser(state: AuthStateModel) {
        console.log('getuser');

        return state.user;
    }

    @Selector()
    static getShowUsername(state: AuthStateModel) {
        return state.showUsername;
    }

    @Action(CompleteSignin)
    completeSignin(ctx: StateContext<AuthState>) {
        console.log('complete signin');

        this._authService.completeAuthentication();
    }

    @Action(UpdateUser)
    updateUser(ctx: StateContext<AuthStateModel>, payload: UpdateUser) {
        console.log('update user');

        ctx.patchState({
            user: payload.user
        });
    }

    @Action(ShowUsername)
    showUsername(ctx: StateContext<AuthStateModel>, payload: ShowUsername) {
        ctx.patchState({
            showUsername: payload.doShowUsername
        });
    }
}
