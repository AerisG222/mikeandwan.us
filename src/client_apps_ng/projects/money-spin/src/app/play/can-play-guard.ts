import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { StateService } from '../services/state.service';

@Injectable()
export class CanPlayGuard  {
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
