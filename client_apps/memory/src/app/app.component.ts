import { Component } from '@angular/core';
import { ROUTER_DIRECTIVES } from '@angular/router';

@Component({
    moduleId: module.id,
    selector: 'memory-app',
    directives: [ ROUTER_DIRECTIVES ],
    templateUrl: 'app.component.html',
    styleUrls: [ 'app.component.css' ]
})
export class MemoryAppComponent {

}
