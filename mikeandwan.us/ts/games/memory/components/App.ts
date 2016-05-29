import { Component } from '@angular/core';
import { RouterOutlet, RouteConfig } from '@angular/router-deprecated';
import { Play } from './Play';
import { ChooseTurtle } from './ChooseTurtle';

@Component({
    selector: 'memory',	
    directives: [ RouterOutlet ],
    template: '<router-outlet></router-outlet>'
})
@RouteConfig([
    { path: '/',     name: 'ChooseTurtle', component: ChooseTurtle },
    { path: '/play', name: 'Play',         component: Play }
])
// TODO: add otherwise route config item
export class MawMemoryApp {
    
}
