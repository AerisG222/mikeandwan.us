import { bootstrap } from '@angular/platform-browser-dynamic';
import { enableProdMode } from '@angular/core';

import { AppComponent, environment } from './app/';
import {  } from './app/environment';

if (environment.production) {
    enableProdMode();
}

bootstrap(AppComponent).catch(err => console.error(err));
