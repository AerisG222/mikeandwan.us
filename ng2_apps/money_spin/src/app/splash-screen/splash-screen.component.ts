import { Component, AfterViewInit } from '@angular/core';
import { Router } from '@angular/router';
import { StateService } from '../state.service';

@Component({
  moduleId: module.id,
  selector: 'app-splash-screen',
  templateUrl: 'splash-screen.component.html',
  styleUrls: ['splash-screen.component.css']
})
export class SplashScreenComponent implements AfterViewInit {
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
