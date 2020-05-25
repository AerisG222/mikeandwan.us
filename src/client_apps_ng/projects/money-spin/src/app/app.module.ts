import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { ChoosePlayerComponent } from './choose-player/choose-player.component';
import { PlayComponent } from './play/play.component';
import { PlayerScoreComponent } from './player-score/player-score.component';
import { PlayerSelectComponent } from './player-select/player-select.component';
import { SpinnerComponent } from './spinner/spinner.component';
import { SplashScreenComponent } from './splash-screen/splash-screen.component';
import { WinnerComponent } from './winner/winner.component';
import { CanPlayGuard } from './play/can-play-guard';
import { StateService } from './services/state.service';

@NgModule({
    imports: [
        BrowserModule,
        RouterModule.forRoot([
            { path: '',       component: SplashScreenComponent },
            { path: 'choose', component: ChoosePlayerComponent },
            { path: 'play',   component: PlayComponent, canActivate: [ CanPlayGuard ] },
            { path: 'winner', component: WinnerComponent },
            { path: '**',     redirectTo: '/' }
        ])
    ],
    declarations: [
        AppComponent,
        ChoosePlayerComponent,
        PlayComponent,
        PlayerScoreComponent,
        PlayerSelectComponent,
        SpinnerComponent,
        SplashScreenComponent,
        WinnerComponent
    ],
    providers: [
        CanPlayGuard,
        StateService
    ],
    bootstrap: [
        AppComponent
    ]
})
export class AppModule { }
