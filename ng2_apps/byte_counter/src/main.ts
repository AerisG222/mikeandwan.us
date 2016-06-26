import { bootstrap } from '@angular/platform-browser-dynamic';
import { enableProdMode } from '@angular/core';

import { ByteCounterAppComponent } from './app/byte-counter.component';
import { environment } from './app/environment';

if (environment.production) {
    enableProdMode();
}

bootstrap(ByteCounterAppComponent).catch(err => console.error(err));
