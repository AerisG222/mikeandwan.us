import { Component } from '@angular/core';

import { Player } from '../player.model';
import { StateService } from '../state.service';

@Component({
    selector: 'app-play',
    templateUrl: 'play.component.html',
    styleUrls: [ 'play.component.css' ]
})
export class PlayComponent {
    player1: Player;
    player2: Player;
    currentPlayer: Player;

    constructor(private _stateService: StateService) {
        this.player1 = this._stateService.player1;
        this.player2 = this._stateService.player2;

        this.updateCurrentPlayer();
    }

    addScore(score: number): void {
        let isGameOver = this._stateService.evaluateTurn(score);

        if (!isGameOver) {
            this.updateCurrentPlayer();
        }
    }

    private updateCurrentPlayer(): void {
        this.currentPlayer = this._stateService.currentPlayer;
    }
}
