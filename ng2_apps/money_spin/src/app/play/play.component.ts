import { Component } from '@angular/core';
import { NgIf } from '@angular/common';

import { Player, StateService } from '../';
import { SpinnerComponent } from '../spinner';
import { PlayerScoreComponent } from '../player-score';

@Component({
    moduleId: module.id,
    selector: 'app-play',
    directives: [NgIf, SpinnerComponent, PlayerScoreComponent],
    templateUrl: 'play.component.html',
    styleUrls: ['play.component.css']
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
