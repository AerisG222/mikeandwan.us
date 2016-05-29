import { bootstrap, BrowserDomAdapter } from '@angular/platform-browser-dynamic';
import { enableProdMode } from '@angular/core';
import { HTTP_BINDINGS } from '@angular/http';
import { ROUTER_PROVIDERS } from '@angular/router';
import { LocalStorageService, BreadcrumbService, ResponsiveService } from '../../ng_maw/src/app/shared';
import 'rxjs/add/operator/map';
import { PhotoDataService, PhotoNavigationService, PhotoSourceFactory, PhotoStateService } from './app/shared';
import { PhotosAppComponent, environment } from './app/';


if (environment.production) {
  enableProdMode();
}

bootstrap(PhotosAppComponent, [ 
    HTTP_BINDINGS, 
    ROUTER_PROVIDERS,
    BrowserDomAdapter,
    LocalStorageService, 
    BreadcrumbService,
    ResponsiveService,
    PhotoDataService,
    PhotoNavigationService,
    PhotoStateService,
    PhotoSourceFactory ]);

