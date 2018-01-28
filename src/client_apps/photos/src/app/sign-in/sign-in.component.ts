import { Component } from '@angular/core';
import { AuthService } from '../shared/auth-service';
import { PhotoNavigationService } from '../shared/photo-navigation.service';

@Component({
    selector: 'app-sign-in',
    templateUrl: './sign-in.component.html',
    styleUrls: ['./sign-in.component.css']
})
export class SignInComponent {

    constructor(auth: AuthService,
                navService: PhotoNavigationService) {
        auth.completeLogin()
            .subscribe(user => {
                if (user) {
                    navService.gotoModeList();
                }
            });
    }
}
