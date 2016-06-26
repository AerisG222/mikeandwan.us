import { bootstrap } from '@angular/platform-browser-dynamic';
import { enableProdMode } from '@angular/core';

import { GooglemapsAppComponent } from './app/googlemaps.component';
import { environment } from './app/environment';

if (environment.production) {
    enableProdMode();
}

bootstrap(GooglemapsAppComponent).catch(err => console.error(err));
