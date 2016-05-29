import { bootstrap } from '@angular/platform-browser-dynamic';
import { HTTP_BINDINGS } from '@angular/http';
import { ROUTER_PROVIDERS } from '@angular/router-deprecated';
import { MawVideoApp } from './components/App';
import { LocalStorageService } from '../ng_maw/services/LocalStorageService';
import { BreadcrumbService } from '../ng_maw/services/BreadcrumbService';
import { ResponsiveService } from '../ng_maw/services/ResponsiveService';
import { SizeService } from './services/SizeService';
import { VideoDataService } from './services/VideoDataService';
import { VideoNavigationService } from './services/VideoNavigationService';
import { VideoStateService } from './services/VideoStateService';

bootstrap(MawVideoApp,[ HTTP_BINDINGS, 
                        ROUTER_PROVIDERS, 
                        LocalStorageService, 
                        BreadcrumbService,
                        ResponsiveService, 
                        SizeService, 
                        VideoDataService, 
                        VideoNavigationService,
                        VideoStateService ])
    .catch(err => console.error(err));
