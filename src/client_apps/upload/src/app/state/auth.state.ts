import { Router } from '@angular/router';
import { State, Action, StateContext, Selector } from '@ngxs/store';
import { User } from 'oidc-client';
import { CompleteSignin, UpdateUser, ShowUsername } from './auth.actions';
import { AuthService } from '../services/auth-service';

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
    constructor(private _authService: AuthService) {

    }

    @Selector()
    static getUser(state: AuthStateModel) {
        return state.user;
    }

    @Selector()
    static getShowUsername(state: AuthStateModel) {
        return state.showUsername;
    }

    @Action(CompleteSignin)
    completeSignin(ctx: StateContext<AuthState>) {
        this._authService.completeAuthentication();
    }

    @Action(UpdateUser)
    updateUser(ctx: StateContext<AuthStateModel>, user: User) {
        ctx.patchState({
            user: user
        });
    }

    @Action(ShowUsername)
    showUsername(ctx: StateContext<AuthStateModel>, doShowUsername: boolean) {
        ctx.patchState({
            showUsername: doShowUsername
        });
    }
}
