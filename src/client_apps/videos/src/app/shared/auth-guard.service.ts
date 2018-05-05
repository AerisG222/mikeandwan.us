import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from './auth-service';
import { Observable } from 'rxjs';

@Injectable()
export class AuthGuardService implements CanActivate {
    constructor(private _authService: AuthService) {

    }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | Observable<boolean> | Promise<boolean> {
        if (this._authService.isLoggedIn()) {
            return true;
        }

        return new Promise((resolve) => {
            this._authService.attemptSilentSignin()
                .then(() => {
                    if (this._authService.isLoggedIn()) {
                        resolve(true);
                    } else {
                        this._authService.startAuthentication();
                        resolve(false);
                    }
                })
                .catch(() => {
                    this._authService.startAuthentication();
                    resolve(false);
                });
        });
    }
}
