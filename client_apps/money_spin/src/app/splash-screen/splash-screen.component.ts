import { Component, AfterViewInit } from '@angular/core';

import { StateService } from '../state.service';

@Component({
    moduleId: module.id,
    selector: 'splash-screen',
    templateUrl: 'splash-screen.component.html',
    styleUrls: [ 'splash-screen.component.css' ]
})
export class SplashScreenComponent implements AfterViewInit {
    constructor(private _stateService: StateService) {
        _stateService.setSplashShown();
    }

    ngAfterViewInit(): void {
        setTimeout(() => {
            this._stateService.newGame();
        }, 2400);
    }
}