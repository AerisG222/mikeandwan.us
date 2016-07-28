import { bootstrap } from '@angular/platform-browser-dynamic';
import { enableProdMode } from '@angular/core';
import { disableDeprecatedForms, provideForms } from '@angular/forms';

import { ByteCounterAppComponent } from './app/app.component';
import { environment } from './app/environment';

if (environment.production) {
    enableProdMode();
}

bootstrap(ByteCounterAppComponent, [
    disableDeprecatedForms(),
    provideForms(),
]).catch(err => console.error(err));
