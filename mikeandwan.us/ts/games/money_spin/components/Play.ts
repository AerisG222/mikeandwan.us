import { Component } from '@angular/core';
import { NgIf } from '@angular/common';
import { StateService } from '../services/StateService';
import { Spinner } from './Spinner';
import { PlayerScore } from './PlayerScore';
import { Player } from '../models/Player';

@Component({
    selector: 'play',
    directives: [ NgIf, Spinner, PlayerScore ],
    templateUrl: '/js/games/money_spin/components/Play.html'
})
export class Play {
    player1 : Player;
    player2 : Player;
    currentPlayer : Player;
    
    constructor(private _stateService : StateService) {
        this.player1 = this._stateService.player1;
        this.player2 = this._stateService.player2;
        
        this.updateCurrentPlayer();
    }
    
    addScore(score : number) : void {
        let isGameOver = this._stateService.evaluateTurn(score);
        
        if(!isGameOver) {
            this.updateCurrentPlayer();
        }
    }
    
    private updateCurrentPlayer() : void {
        this.currentPlayer = this._stateService.currentPlayer;
    }
}
