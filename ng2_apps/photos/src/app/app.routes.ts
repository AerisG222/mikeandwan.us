import { provideRouter, RouterConfig, Data } from '@angular/router';

import { ModeComponent } from './mode/mode.component';
import { CategoryListComponent } from './category-list/category-list.component';
import { PhotoListComponent } from './photo-list/photo-list.component';
import { ModeRouteInfo } from './shared/mode-route-info.model';
import { RouteMode } from './shared/route-mode.model';

// TODO: add otherwise route config item
export const routes: RouterConfig = [
    { path: 'photos',                      component: ModeComponent },
    { path: 'photos/random',               component: PhotoListComponent,   data: new ModeRouteInfo(RouteMode.Random) }, 
    { path: 'photos/year/:year',           component: CategoryListComponent },
    { path: 'photos/year/:year/:category', component: PhotoListComponent,   data: new ModeRouteInfo(RouteMode.Category) }, 
    { path: 'photos/comment/:type/:order', component: PhotoListComponent,   data: new ModeRouteInfo(RouteMode.Comment) }, 
    { path: 'photos/rating/:type/:order',  component: PhotoListComponent,   data: new ModeRouteInfo(RouteMode.Rating) }, 
    { path: 'photos/:mode',                component: ModeComponent }
];

export const APP_ROUTER_PROVIDERS = [
    provideRouter(routes)
];
