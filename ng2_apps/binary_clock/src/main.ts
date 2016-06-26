import { bootstrap } from '@angular/platform-browser-dynamic';
import { enableProdMode } from '@angular/core';

import { BinaryClockAppComponent } from './app/binary-clock.component';
import { environment } from './app/environment';

if (environment.production) {
    enableProdMode();
}

bootstrap(BinaryClockAppComponent).catch(err => console.error(err));
