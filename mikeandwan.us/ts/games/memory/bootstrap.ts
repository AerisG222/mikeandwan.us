import { bootstrap } from '@angular/platform-browser-dynamic';
import { ROUTER_PROVIDERS } from '@angular/router-deprecated';
import { MawMemoryApp } from './components/App';
import { MemoryService } from './services/MemoryService';

bootstrap(MawMemoryApp, [ MemoryService, 
                          ROUTER_PROVIDERS ])
    .catch(err => console.error(err));
