import { bootstrap } from '@angular/platform-browser-dynamic';
import { enableProdMode } from '@angular/core';

import { NgMawAppComponent } from './app/ng-maw.component';
import { environment } from './app/environment';

if (environment.production) {
    enableProdMode();
}

bootstrap(NgMawAppComponent).catch(err => console.error(err));
