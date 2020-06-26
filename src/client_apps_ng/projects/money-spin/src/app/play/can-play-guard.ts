import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { StateService } from '../services/state.service';

@Injectable()
export class CanPlayGuard implements CanActivate {
    constructor(private router: Router,
                private stateService: StateService) {

    }

    canActivate(): boolean {
        if (this.stateService.isReadyToPlay()) {
            return true;
        }

        this.router.navigateByUrl('/');

        return false;
    }
}
