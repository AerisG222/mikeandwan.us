import { Component, OnInit } from '@angular/core';
import { PhotoNavigationService } from '../services/photo-navigation.service';
import { AuthService } from 'maw-auth';

@Component({
    selector: 'app-sign-in',
    templateUrl: './sign-in.component.html',
    styleUrls: ['./sign-in.component.css']
})
export class SignInComponent implements OnInit {

    constructor(private _authService: AuthService,
                private _navService: PhotoNavigationService) {

    }

    ngOnInit() {
        this._authService.completeAuthentication().then(() => {
            this._navService.gotoModeList();
        });
    }
}
