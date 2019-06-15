import { Component, AfterViewInit } from '@angular/core';

import { StateService } from '../services/state.service';

@Component({
    selector: 'app-splash-screen',
    templateUrl: './splash-screen.component.html',
    styleUrls: [ './splash-screen.component.scss' ]
})
export class SplashScreenComponent implements AfterViewInit {
    constructor(private stateService: StateService) {
        stateService.setSplashShown();
    }

    ngAfterViewInit(): void {
        setTimeout(() => {
            this.stateService.newGame();
        }, 2400);
    }
}
