import { Component } from '@angular/core';

import { ICharacter } from '../models/icharacter.model';
import { MemoryService } from '../services/memory.service';

@Component({
    selector: 'app-choose-turtle',
    templateUrl: './choose-turtle.component.html',
    styleUrls: [ './choose-turtle.component.scss' ]
})
export class ChooseTurtleComponent {
    private character1?: ICharacter;
    private character2?: ICharacter;

    constructor(private svc: MemoryService) {

    }

    setCharacter1(character: ICharacter): void {
        this.character1 = character;

        this.svc.player1 = {
            character,
            isPlayersTurn: false,
            score: 0
        };
    }

    setCharacter2(character: ICharacter): void {
        this.character2 = character;

        this.svc.player2 = {
            character,
            isPlayersTurn: false,
            score: 0
        };
    }

    get readyToPlay(): boolean {
        return this.character1 != null && this.character2 != null;
    }

    play(): void {
        this.svc.startGame();
    }
}
