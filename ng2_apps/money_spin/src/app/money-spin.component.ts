import { Component } from '@angular/core';
import { Routes, Route, ROUTER_DIRECTIVES } from '@angular/router';
import { SplashScreenComponent } from './splash-screen/splash-screen.component'
import { ChoosePlayerComponent } from './choose-player/choose-player.component'
import { PlayComponent } from './play/play.component'
import { WinnerComponent } from './winner/winner.component'

@Component({
    moduleId: module.id,
    selector: 'money-spin-app',
    directives: [ ROUTER_DIRECTIVES ],
    templateUrl: 'money-spin.component.html',
    styleUrls: ['money-spin.component.css']
})
@Routes([
    new Route({ path: '/',       component: SplashScreenComponent }),
    new Route({ path: '/choose', component: ChoosePlayerComponent }),
    new Route({ path: '/play',   component: PlayComponent }),
    new Route({ path: '/winner', component: WinnerComponent })
])
export class MoneySpinAppComponent {
    
}
