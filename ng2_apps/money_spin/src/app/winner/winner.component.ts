import { Component } from '@angular/core';

import { Player, StateService } from '../';

@Component({
  moduleId: module.id,
  selector: 'app-winner',
  templateUrl: 'winner.component.html',
  styleUrls: ['winner.component.css']
})
export class WinnerComponent {
    winner : Player;
    
    constructor(private _stateService : StateService) {
        this.winner = this._stateService.currentPlayer;
    }
    
    newGame() : void {
        this._stateService.newGame();
    }
}
