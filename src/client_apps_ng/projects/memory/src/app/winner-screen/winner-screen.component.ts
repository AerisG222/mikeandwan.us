import { Component, Input, Output, EventEmitter } from '@angular/core';

import { IPlayer } from '../models/iplayer.model';

@Component({
    selector: 'app-winner-screen',
    templateUrl: './winner-screen.component.html',
    styleUrls: [ './winner-screen.component.scss' ]
})
export class WinnerScreenComponent {
    @Input() player?: IPlayer;
    @Output() rematch = new EventEmitter<void>();
    @Output() newgame = new EventEmitter<void>();

    onRematch(): void {
        this.rematch.next();
    }

    onNewGame(): void {
        this.newgame.next();
    }
}
