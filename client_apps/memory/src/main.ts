import { bootstrap } from '@angular/platform-browser-dynamic';
import { enableProdMode } from '@angular/core';

import { APP_ROUTER_PROVIDERS } from './app/app.routes';
import { MemoryService } from './app/memory.service';
import { AppComponent, environment } from './app/';
import { CanPlayGuard } from './app/play/can-play-guard';

if (environment.production) {
    enableProdMode();
}

bootstrap(AppComponent, [
    APP_ROUTER_PROVIDERS,
    MemoryService,
    CanPlayGuard
])
.catch(err => console.error(err));
