import { bootstrap } from '@angular/platform-browser-dynamic';
import { enableProdMode } from '@angular/core';
import { GooglemapsAppComponent, environment } from './app/';

if (environment.production) {
  enableProdMode();
}

bootstrap(GooglemapsAppComponent);

