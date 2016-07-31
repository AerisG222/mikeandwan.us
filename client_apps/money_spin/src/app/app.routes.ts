import { provideRouter, RouterConfig } from '@angular/router';

import { SplashScreenComponent } from './splash-screen/splash-screen.component';
import { ChoosePlayerComponent } from './choose-player/choose-player.component';
import { PlayComponent } from './play/play.component';
import { WinnerComponent } from './winner/winner.component';
import { CanPlayGuard } from './play/can-play-guard';

export const routes: RouterConfig = [
    { path: '',       component: SplashScreenComponent },
    { path: 'choose', component: ChoosePlayerComponent },
    { path: 'play',   component: PlayComponent, canActivate: [ CanPlayGuard ] },
    { path: 'winner', component: WinnerComponent },
    { path: '**',     redirectTo: '/' },
];

export const APP_ROUTER_PROVIDERS = [
    provideRouter(routes)
];
