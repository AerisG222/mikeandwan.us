import { Component, OnInit } from '@angular/core';
import { Store } from '@ngxs/store';
import { CompleteSignin } from '../state/auth.actions';

@Component({
    selector: 'app-sign-in',
    templateUrl: './sign-in.component.html',
    styleUrls: ['./sign-in.component.css']
})
export class SignInComponent implements OnInit {

    constructor(private _store: Store) {

    }

    ngOnInit() {
        this._store.dispatch(new CompleteSignin());
    }
}
