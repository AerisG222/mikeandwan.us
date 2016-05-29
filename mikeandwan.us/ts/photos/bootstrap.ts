import { bootstrap, BrowserDomAdapter } from '@angular/platform-browser-dynamic';
import { HTTP_BINDINGS } from '@angular/http';
import { ROUTER_PROVIDERS } from '@angular/router-deprecated';
import { MawPhotoApp } from './components/App';
import { LocalStorageService } from '../ng_maw/services/LocalStorageService';
import { BreadcrumbService } from '../ng_maw/services/BreadcrumbService';
import { ResponsiveService } from '../ng_maw/services/ResponsiveService';
import { PhotoDataService } from './services/PhotoDataService';
import { PhotoNavigationService } from './services/PhotoNavigationService';
import { PhotoStateService } from './services/PhotoStateService';
import { PhotoSourceFactory } from './models/PhotoSourceFactory';
import 'rxjs/add/operator/map';

bootstrap(MawPhotoApp, [ HTTP_BINDINGS, 
                         ROUTER_PROVIDERS,
                         BrowserDomAdapter,
                         LocalStorageService, 
                         BreadcrumbService,
                         ResponsiveService,
                         PhotoDataService,
                         PhotoNavigationService,
                         PhotoStateService,
                         PhotoSourceFactory ])
    .catch(err => console.error(err));
