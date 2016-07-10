import { bootstrap } from '@angular/platform-browser-dynamic';
import { enableProdMode } from '@angular/core';

import { FilesizeAppComponent } from './app/app.component';
import { environment } from './app/environment';

if (environment.production) {
    enableProdMode();
}

bootstrap(FilesizeAppComponent).catch(err => console.error(err));
