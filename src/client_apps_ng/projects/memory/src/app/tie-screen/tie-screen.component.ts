import { Component, Input, Output, EventEmitter } from '@angular/core';

import { IPlayer } from '../models/iplayer.model';

@Component({
    selector: 'app-tie-screen',
    templateUrl: './tie-screen.component.html',
    styleUrls: [ './tie-screen.component.scss' ]
})
export class TieScreenComponent {
    @Input() player1?: IPlayer;
    @Input() player2?: IPlayer;
    @Output() rematch = new EventEmitter<void>();
    @Output() newgame = new EventEmitter<void>();

    onRematch(): void {
        this.rematch.next();
    }

    onNewGame(): void {
        this.newgame.next();
    }
}
