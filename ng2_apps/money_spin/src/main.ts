import { bootstrap } from '@angular/platform-browser-dynamic';
import { enableProdMode } from '@angular/core';
import { ROUTER_PROVIDERS } from '@angular/router';
import { MoneySpinAppComponent, environment, StateService } from './app/';

if (environment.production) {
  enableProdMode();
}

bootstrap(MoneySpinAppComponent, [ ROUTER_PROVIDERS, StateService ]);

