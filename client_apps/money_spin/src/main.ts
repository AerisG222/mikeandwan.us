import { bootstrap } from '@angular/platform-browser-dynamic';
import { enableProdMode } from '@angular/core';

import { APP_ROUTER_PROVIDERS } from './app/app.routes';
import { MoneySpinAppComponent } from './app/app.component';
import { environment } from './app/environment';
import { StateService } from './app/state.service';
import { CanPlayGuard } from './app/play/can-play-guard';

if (environment.production) {
    enableProdMode();
}

bootstrap(MoneySpinAppComponent, [
    APP_ROUTER_PROVIDERS,
    StateService,
    CanPlayGuard
])
.catch(err => console.error(err));
