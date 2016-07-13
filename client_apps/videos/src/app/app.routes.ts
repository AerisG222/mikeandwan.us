import { provideRouter, RouterConfig } from '@angular/router';

import { YearListComponent } from './year-list/year-list.component';
import { CategoryListComponent } from './category-list/category-list.component';
import { VideoListComponent } from './video-list/video-list.component';

// TODO: add otherwise route config item
export const routes: RouterConfig = [
    { path: '',                component: YearListComponent },
    { path: ':year',           component: CategoryListComponent },
    { path: ':year/:category', component: VideoListComponent },
    { path: '**',              redirectTo: '/' },
];

export const APP_ROUTER_PROVIDERS = [
    provideRouter(routes)
];
