import { Component } from '@angular/core';

import { Player } from '../models/player.model';
import { StateService } from '../services/state.service';

@Component({
    selector: 'app-winner',
    templateUrl: './winner.component.html',
    styleUrls: [ './winner.component.scss' ]
})
export class WinnerComponent {
    winner: Player;

    constructor(private stateService: StateService) {
        this.winner = this.stateService.currentPlayer as Player;
    }

    newGame(): void {
        this.stateService.newGame();
    }
}
