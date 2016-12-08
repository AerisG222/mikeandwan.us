import { Component, Input, Output, EventEmitter } from '@angular/core';

import { IPlayer } from '../iplayer.model';

@Component({
    selector: 'app-winner-screen',
    templateUrl: './winner-screen.component.html',
    styleUrls: [ './winner-screen.component.css' ]
})
export class WinnerScreenComponent {
    @Input() player: IPlayer;
    @Output() rematch: EventEmitter<any> = new EventEmitter<any>();
    @Output() newgame: EventEmitter<any> = new EventEmitter<any>();

    onRematch(): void {
        this.rematch.next(null);
    }

    onNewGame(): void {
        this.newgame.next(null);
    }
}
