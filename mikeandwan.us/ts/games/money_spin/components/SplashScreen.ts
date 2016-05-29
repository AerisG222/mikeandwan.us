import { Component, AfterViewInit } from '@angular/core';
import { Router } from '@angular/router-deprecated';
import { StateService } from '../services/StateService';

@Component({
    selector: 'splash',	
    templateUrl: '/js/games/money_spin/components/SplashScreen.html'
})
export class SplashScreen implements AfterViewInit {
    constructor(private _router : Router,
                private _stateService : StateService) {
        _stateService.setSplashShown();
    }
    
    ngAfterViewInit() : void {
        setTimeout(() => {
            this._router.navigate(['ChoosePlayer']);
        }, 2400);
    }
}
