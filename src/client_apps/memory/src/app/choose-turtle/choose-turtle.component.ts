import { Component } from '@angular/core';

import { ICharacter } from '../models/icharacter.model';
import { MemoryService } from '../services/memory.service';

@Component({
    selector: 'app-choose-turtle',
    templateUrl: './choose-turtle.component.html',
    styleUrls: [ './choose-turtle.component.scss' ]
})
export class ChooseTurtleComponent {
    private character1: ICharacter = null;
    private character2: ICharacter = null;

    constructor(private svc: MemoryService) {

    }

    setCharacter1(character: ICharacter) {
        this.character1 = character;
        this.svc.player1.character = character;
    }

    setCharacter2(character: ICharacter) {
        this.character2 = character;
        this.svc.player2.character = character;
    }

    get readyToPlay(): boolean {
        return this.character1 != null && this.character2 != null;
    }

    play(): void {
        this.svc.startGame();
    }
}
