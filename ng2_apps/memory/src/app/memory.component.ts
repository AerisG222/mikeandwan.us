import { Component } from '@angular/core';
import { RouteConfig, RouterOutlet, ROUTER_PROVIDERS } from '@angular/router-deprecated';

import { PlayComponent } from './play';
import { ChooseTurtleComponent } from './choose-turtle';

@RouteConfig([
    { path: '/',     name: 'ChooseTurtle', component: ChooseTurtleComponent },
    { path: '/play', name: 'Play',         component: PlayComponent }
])
@Component({
    moduleId: module.id,
    selector: 'memory-app',
    directives: [RouterOutlet],
    templateUrl: 'memory.component.html',
    styleUrls: ['memory.component.css']
})
export class MemoryAppComponent {

}
