import { bootstrap } from '@angular/platform-browser-dynamic';
import { HTTP_BINDINGS } from '@angular/http';
import { enableProdMode } from '@angular/core';
import { disableDeprecatedForms, provideForms } from '@angular/forms';

import { BreadcrumbService } from './ng_maw/shared/breadcrumb.service';
import { LocalStorageService } from './ng_maw/shared/local-storage.service';
import { ResponsiveService } from './ng_maw/shared/responsive.service';

import { APP_ROUTER_PROVIDERS } from './app/app.routes';
import { AppComponent, environment } from './app/';
import { SizeService } from './app/shared/size.service';
import { VideoDataService } from './app/shared/video-data.service';
import { VideoNavigationService } from './app/shared/video-navigation.service';
import { VideoStateService } from './app/shared/video-state.service';

if (environment.production) {
    enableProdMode();
}

bootstrap(AppComponent, [
    HTTP_BINDINGS,
    APP_ROUTER_PROVIDERS,
    disableDeprecatedForms(),
    provideForms(),
    LocalStorageService,
    BreadcrumbService,
    ResponsiveService,
    SizeService,
    VideoDataService,
    VideoNavigationService,
    VideoStateService
]).catch(err => console.error(err));
