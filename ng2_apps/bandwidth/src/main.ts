import { bootstrap } from '@angular/platform-browser-dynamic';
import { enableProdMode } from '@angular/core';

import { BandwidthAppComponent } from './app/bandwidth.component';
import { environment } from './app/environment';

if (environment.production) {
    enableProdMode();
}

bootstrap(BandwidthAppComponent).catch(err => console.error(err));
