import { bootstrap } from '@angular/platform-browser-dynamic';
import { enableProdMode } from '@angular/core';

import { APP_ROUTER_PROVIDERS } from './app/app.routes';
import { AppComponent, environment } from './app/';
import {  } from './app/environment';
import { StateService } from './app/state.service';
import { CanPlayGuard } from './app/play/can-play-guard';

if (environment.production) {
    enableProdMode();
}

bootstrap(AppComponent, [
    APP_ROUTER_PROVIDERS,
    StateService,
    CanPlayGuard
])
.catch(err => console.error(err));
