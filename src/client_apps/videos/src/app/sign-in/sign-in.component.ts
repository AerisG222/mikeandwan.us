import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth-service';
import { VideoNavigationService } from '../services/video-navigation.service';

@Component({
    selector: 'app-sign-in',
    templateUrl: './sign-in.component.html',
    styleUrls: ['./sign-in.component.css']
})
export class SignInComponent implements OnInit {

    constructor(private _authService: AuthService,
                private _navService: VideoNavigationService) {

    }

    ngOnInit() {
        this._authService.completeAuthentication().then(() => {
            this._navService.gotoYearList();
        });
    }
}
