import { Component, OnInit } from '@angular/core';
import { Store, Select } from '@ngxs/store';
import { CompleteSignin } from '../state/auth.actions';
import { AuthState } from '../state/auth.state';
import { Observable } from 'rxjs';
import { User } from 'oidc-client';
import { Router } from '@angular/router';

@Component({
    selector: 'app-sign-in',
    templateUrl: './sign-in.component.html',
    styleUrls: ['./sign-in.component.css']
})
export class SignInComponent implements OnInit {
    @Select(AuthState.getUser) _user$: Observable<User>;

    constructor(private _router: Router,
                private _store: Store) {

    }

    ngOnInit() {
        this._user$.subscribe(user => {
            if (user == null || user === undefined) {
                return;
            }

            this._router.navigate([ '/' ]);
        });

        this._store.dispatch(new CompleteSignin());
    }
}
