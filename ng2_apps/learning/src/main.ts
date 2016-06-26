import { bootstrap } from '@angular/platform-browser-dynamic';
import { enableProdMode } from '@angular/core';

import { LearningAppComponent } from './app/learning.component';
import { environment } from './app/environment';

if (environment.production) {
    enableProdMode();
}

bootstrap(LearningAppComponent).catch(err => console.error(err));
