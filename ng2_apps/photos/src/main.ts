import { bootstrap } from '@angular/platform-browser-dynamic';
import { enableProdMode } from '@angular/core';
import { HTTP_BINDINGS } from '@angular/http';
import 'rxjs/add/operator/map';

import { LocalStorageService } from '../../ng_maw/src/app/shared/local-storage.service';
import { BreadcrumbService } from '../../ng_maw/src/app/shared/breadcrumb.service';
import { ResponsiveService } from '../../ng_maw/src/app/shared/responsive.service';

import { APP_ROUTER_PROVIDERS } from './app/app.routes';
import { PhotosAppComponent } from './app/photos.component';
import { environment } from './app/environment';
import { PhotoDataService } from './app/shared/photo-data.service';
import { PhotoNavigationService } from './app/shared/photo-navigation.service';
import { PhotoSourceFactory } from './app/shared/photo-source-factory.model';
import { PhotoStateService } from './app/shared/photo-state.service';

if (environment.production) {
    enableProdMode();
}

bootstrap(PhotosAppComponent, [
    HTTP_BINDINGS,
    APP_ROUTER_PROVIDERS,
    LocalStorageService,
    BreadcrumbService,
    ResponsiveService,
    PhotoDataService,
    PhotoNavigationService,
    PhotoStateService,
    PhotoSourceFactory])
.catch(err => console.error(err));
