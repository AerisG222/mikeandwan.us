import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { StateService } from '../state.service';

@Injectable()
export class CanPlayGuard implements CanActivate {
    constructor(private _router: Router,
                private _stateService: StateService) {

    }

    canActivate() {
        if (this._stateService.isReadyToPlay()) {
            return true;
        }

        this._router.navigateByUrl('/');
        
        return false;
    }
}
