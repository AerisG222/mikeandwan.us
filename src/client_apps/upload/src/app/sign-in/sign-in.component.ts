import { Component, OnInit } from '@angular/core';
import { AuthService } from '../shared/auth-service';
import { Router } from '@angular/router';

@Component({
    selector: 'app-sign-in',
    templateUrl: './sign-in.component.html',
    styleUrls: ['./sign-in.component.css']
})
export class SignInComponent implements OnInit {

    constructor(private _authService: AuthService,
                private _router: Router) {

    }

    ngOnInit() {
        this._authService.completeAuthentication().then(() => {
            this._router.navigate([ '/' ]);
        });
    }
}