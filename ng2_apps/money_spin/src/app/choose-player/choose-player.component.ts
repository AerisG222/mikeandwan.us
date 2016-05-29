import { Component } from '@angular/core';
import { StateService } from '../state.service';
import { Character } from '../character';
import { PlayerSelectComponent } from '../player-select/player-select.component';

@Component({
    moduleId: module.id,
    selector: 'app-choose-player',
    directives: [ PlayerSelectComponent ],
    templateUrl: 'choose-player.component.html',
    styleUrls: ['choose-player.component.css']
})
export class ChoosePlayerComponent {
    player1characters : Array<Character>;
    player2characters : Array<Character>;
    isReadyToPlay = false;
    
    constructor(private _stateService : StateService) {
        this.player1characters = this._stateService.player1Characters;
        this.player2characters = this._stateService.player2Characters;
    }
    
    player1Selected(character : Character) : void {
        this._stateService.setPlayer1(character);
        this.setIsReadyToPlay();
    }
    
    player2Selected(character : Character) : void {
        this._stateService.setPlayer2(character);
        this.setIsReadyToPlay();
    }
    
    play() : void {
        this._stateService.startGame();
    }
    
    private setIsReadyToPlay() : void {
        this.isReadyToPlay = this._stateService.player1 != null && this._stateService.player2 != null;
    }
}
