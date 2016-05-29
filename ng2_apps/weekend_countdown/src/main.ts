import { bootstrap } from '@angular/platform-browser-dynamic';
import { enableProdMode } from '@angular/core';
import { WeekendCountdownAppComponent, environment } from './app/';

if (environment.production) {
  enableProdMode();
}

bootstrap(WeekendCountdownAppComponent);

