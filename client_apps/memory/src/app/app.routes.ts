import { provideRouter, RouterConfig } from '@angular/router';

import { PlayComponent } from './play/play.component';
import { ChooseTurtleComponent } from './choose-turtle/choose-turtle.component';

export const routes: RouterConfig = [
    { path: '',     component: ChooseTurtleComponent },
    { path: 'play', component: PlayComponent },
    { path: '**',   redirectTo: '/' },
];

export const APP_ROUTER_PROVIDERS = [
    provideRouter(routes)
];
