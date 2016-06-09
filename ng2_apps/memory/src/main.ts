import { bootstrap } from '@angular/platform-browser-dynamic';
import { enableProdMode } from '@angular/core';
import { ROUTER_PROVIDERS } from '@angular/router-deprecated';

import { MemoryAppComponent, environment, MemoryService } from './app/';


if (environment.production) {
    enableProdMode();
}

bootstrap(MemoryAppComponent, [ROUTER_PROVIDERS, MemoryService]);

