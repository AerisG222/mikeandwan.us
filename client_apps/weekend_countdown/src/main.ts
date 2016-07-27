import { bootstrap } from '@angular/platform-browser-dynamic';
import { enableProdMode } from '@angular/core';

import { WeekendCountdownAppComponent } from './app/app.component';
import { environment } from './app/environment';

if (environment.production) {
    enableProdMode();
}

bootstrap(WeekendCountdownAppComponent);