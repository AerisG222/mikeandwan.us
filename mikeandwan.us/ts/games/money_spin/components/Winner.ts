import { Component } from '@angular/core';
import { StateService } from '../services/StateService';
import { Player } from '../models/Player';

@Component({
    selector: 'winner',
    templateUrl: '/js/games/money_spin/components/Winner.html'
})
export class Winner {
    winner : Player;
    
    constructor(private _stateService : StateService) {
        this.winner = this._stateService.currentPlayer;
    }
    
    newGame() : void {
        this._stateService.newGame();
    }
}
