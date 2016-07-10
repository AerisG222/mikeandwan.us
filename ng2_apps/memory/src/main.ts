import { bootstrap } from '@angular/platform-browser-dynamic';
import { enableProdMode } from '@angular/core';

import { APP_ROUTER_PROVIDERS } from './app/app.routes';
import { MemoryService } from './app/memory.service';
import { MemoryAppComponent } from './app/app.component';
import { environment } from './app/environment';

if (environment.production) {
    enableProdMode();
}

bootstrap(MemoryAppComponent, [
    APP_ROUTER_PROVIDERS,
    MemoryService
])
.catch(err => console.error(err));
