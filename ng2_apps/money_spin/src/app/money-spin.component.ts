import { Component } from '@angular/core';
import { RouteConfig, ROUTER_DIRECTIVES } from '@angular/router-deprecated';

import { SplashScreenComponent } from './splash-screen'
import { ChoosePlayerComponent } from './choose-player'
import { PlayComponent } from './play'
import { WinnerComponent } from './winner'

@Component({
    moduleId: module.id,
    selector: 'money-spin-app',
    directives: [ ROUTER_DIRECTIVES ],
    templateUrl: 'money-spin.component.html',
    styleUrls: [ 'money-spin.component.css' ]
})
@RouteConfig([
    { path: '/',       name: 'SplashScreen', component: SplashScreenComponent },
    { path: '/choose', name: 'ChoosePlayer', component: ChoosePlayerComponent },
    { path: '/play',   name: 'Play',         component: PlayComponent },
    { path: '/winner', name: 'Winner',       component: WinnerComponent }
])
export class MoneySpinAppComponent {

}
