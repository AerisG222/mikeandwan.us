import { Component, Input, Output, EventEmitter } from '@angular/core';

import { IPlayer } from '../models/iplayer.model';

@Component({
    selector: 'app-tie-screen',
    templateUrl: './tie-screen.component.html',
    styleUrls: [ './tie-screen.component.css' ]
})
export class TieScreenComponent {
    @Input() player1: IPlayer;
    @Input() player2: IPlayer;
    @Output() rematch: EventEmitter<any> = new EventEmitter<any>();
    @Output() newgame: EventEmitter<any> = new EventEmitter<any>();

    onRematch(): void {
        this.rematch.next(null);
    }

    onNewGame(): void {
        this.newgame.next(null);
    }
}
