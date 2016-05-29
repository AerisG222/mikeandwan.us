import { bootstrap } from '@angular/platform-browser-dynamic';
import { HTTP_BINDINGS } from '@angular/http';
import { ROUTER_PROVIDERS } from '@angular/router';
import { enableProdMode } from '@angular/core';
import { VideosAppComponent, environment } from './app/';
import { BreadcrumbService, LocalStorageService, ResponsiveService } from '../../ng_maw/src/app/shared/index';
import { SizeService, VideoDataService, VideoNavigationService, VideoStateService } from "./app/shared/index";

if (environment.production) {
  enableProdMode();
}

bootstrap(VideosAppComponent, [
    HTTP_BINDINGS, 
    ROUTER_PROVIDERS, 
    LocalStorageService, 
    BreadcrumbService,
    ResponsiveService, 
    SizeService, 
    VideoDataService, 
    VideoNavigationService,
    VideoStateService
]);
