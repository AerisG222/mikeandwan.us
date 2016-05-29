import { Component } from '@angular/core';
import { StateService } from '../state.service';
import { Player } from '../player';

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
