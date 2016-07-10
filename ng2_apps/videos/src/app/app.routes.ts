import { provideRouter, RouterConfig } from '@angular/router';

import { YearListComponent } from './year-list/year-list.component';
import { CategoryListComponent } from './category-list/category-list.component';
import { VideoListComponent } from './video-list/video-list.component';

// TODO: add otherwise route config item
export const routes: RouterConfig = [
    { path: 'videos',                     component: YearListComponent },
    { path: 'videos/:year',           component: CategoryListComponent },
    { path: 'videos/:year/:category', component: VideoListComponent }
];

export const APP_ROUTER_PROVIDERS = [
    provideRouter(routes)
];
