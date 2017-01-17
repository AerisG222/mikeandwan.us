import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { CardComponent } from './card/card.component';
import { ChooseTurtleComponent } from './choose-turtle/choose-turtle.component';
import { ChooseTurtleGridComponent } from './choose-turtle-grid/choose-turtle-grid.component';
import { GameBoardComponent } from './game-board/game-board.component';
import { PlayComponent } from './play/play.component';
import { TieScreenComponent } from './tie-screen/tie-screen.component';
import { TurtleScoreComponent } from './turtle-score/turtle-score.component';
import { WinnerScreenComponent } from './winner-screen/winner-screen.component';
import { CanPlayGuard } from './play/can-play-guard';
import { MemoryService } from './memory.service';

@NgModule({
    imports: [
        BrowserModule,
        RouterModule.forRoot([
            { path: '',     component: ChooseTurtleComponent },
            { path: 'play', component: PlayComponent, canActivate: [ CanPlayGuard ] },
            { path: '**',   redirectTo: '/' }
        ])
    ],
    declarations: [
        AppComponent,
        CardComponent,
        ChooseTurtleComponent,
        ChooseTurtleGridComponent,
        GameBoardComponent,
        PlayComponent,
        TieScreenComponent,
        TurtleScoreComponent,
        WinnerScreenComponent
    ],
    providers: [
        CanPlayGuard,
        MemoryService
    ],
    bootstrap: [
        AppComponent
    ]
})
export class AppModule { }
