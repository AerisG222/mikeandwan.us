import { Component } from '@angular/core';
import { Routes, Route, RouterOutletMap, ROUTER_PROVIDERS } from '@angular/router';

import { PlayComponent } from './play';
import { ChooseTurtleComponent } from './choose-turtle';

@Routes([
    new Route({ path: '/',     component: ChooseTurtleComponent }),
    new Route({ path: '/play', component: PlayComponent })
])
@Component({
    moduleId: module.id,
    selector: 'memory-app',
    directives: [ RouterOutletMap ],
    templateUrl: 'memory.component.html',
    styleUrls: ['memory.component.css']
})
export class MemoryAppComponent {
  
}
