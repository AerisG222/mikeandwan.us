import { bootstrap } from '@angular/platform-browser-dynamic';
import { enableProdMode } from '@angular/core';

import { APP_ROUTER_PROVIDERS } from './app/app.routes';
import { MoneySpinAppComponent } from './app/money-spin.component';
import { environment } from './app/environment';
import { StateService } from './app/state.service';

if (environment.production) {
    enableProdMode();
}

bootstrap(MoneySpinAppComponent, [
    APP_ROUTER_PROVIDERS, 
    StateService
])
.catch(err => console.error(err));
