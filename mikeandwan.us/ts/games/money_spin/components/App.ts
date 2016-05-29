import { Component } from '@angular/core';
import { RouterOutlet, RouteConfig } from '@angular/router-deprecated';
import { SplashScreen } from './SplashScreen';
import { ChoosePlayer } from './ChoosePlayer';
import { Play } from './Play';
import { Winner } from './Winner';

@Component({
    selector: 'moneyspin',	
    directives: [ RouterOutlet ],
    templateUrl: '/js/games/money_spin/components/App.html'
})
@RouteConfig([
    { path: '/',       name: 'SplashScreen', component: SplashScreen },
    { path: '/choose', name: 'ChoosePlayer', component: ChoosePlayer },
    { path: '/play',   name: 'Play',         component: Play },
    { path: '/winner', name: 'Winner',       component: Winner }
])
export class MoneySpinApp {
    
}
