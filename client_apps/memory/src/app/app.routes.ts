import { provideRouter, RouterConfig } from '@angular/router';

import { PlayComponent } from './play/play.component';
import { ChooseTurtleComponent } from './choose-turtle/choose-turtle.component';
import { CanPlayGuard } from './play/can-play-guard';

export const routes: RouterConfig = [
    { path: '',     component: ChooseTurtleComponent },
    { path: 'play', component: PlayComponent, canActivate: [ CanPlayGuard ] },
    { path: '**',   redirectTo: '/' },
];

export const APP_ROUTER_PROVIDERS = [
    provideRouter(routes)
];
