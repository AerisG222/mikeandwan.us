import { Component } from '@angular/core';
import { ROUTER_DIRECTIVES } from '@angular/router';

import { MemoryService } from './memory.service';

@Component({
    moduleId: module.id,
    selector: 'memory-app',
    directives: [ ROUTER_DIRECTIVES ],
    templateUrl: 'memory.component.html',
    styleUrls: [ 'memory.component.css' ]
})
export class MemoryAppComponent {

}
