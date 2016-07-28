import { bootstrap } from '@angular/platform-browser-dynamic';
import { enableProdMode } from '@angular/core';
import { disableDeprecatedForms, provideForms } from '@angular/forms';

import { FilesizeAppComponent } from './app/app.component';
import { environment } from './app/environment';

if (environment.production) {
    enableProdMode();
}

bootstrap(FilesizeAppComponent, [
    disableDeprecatedForms(),
    provideForms(),
]).catch(err => console.error(err));
