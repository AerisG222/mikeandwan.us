import { Component } from '@angular/core';
import { AuthService } from '../shared/auth-service';
import { VideoNavigationService } from '../shared/video-navigation.service';

@Component({
    selector: 'app-sign-in',
    templateUrl: './sign-in.component.html',
    styleUrls: ['./sign-in.component.css']
})
export class SignInComponent {

    constructor(auth: AuthService,
                navService: VideoNavigationService) {
        auth.completeLogin()
            .subscribe(user => {
                if (user) {
                    navService.gotoYearList();
                }
            });
    }
}
