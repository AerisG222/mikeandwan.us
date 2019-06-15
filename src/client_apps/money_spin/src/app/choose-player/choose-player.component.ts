import { Component } from '@angular/core';

import { Character } from '../models/character.model';
import { StateService } from '../services/state.service';

@Component({
    selector: 'app-choose-player',
    templateUrl: './choose-player.component.html',
    styleUrls: [ './choose-player.component.scss' ]
})
export class ChoosePlayerComponent {
    player1characters: Array<Character>;
    player2characters: Array<Character>;
    isReadyToPlay = false;

    constructor(private stateService: StateService) {
        this.player1characters = this.stateService.player1Characters;
        this.player2characters = this.stateService.player2Characters;
    }

    player1Selected(character: Character): void {
        this.stateService.setPlayer1(character);
        this.setIsReadyToPlay();
    }

    player2Selected(character: Character): void {
        this.stateService.setPlayer2(character);
        this.setIsReadyToPlay();
    }

    play(): void {
        this.stateService.startGame();
    }

    private setIsReadyToPlay(): void {
        this.isReadyToPlay = this.stateService.player1 != null && this.stateService.player2 != null;
    }
}
