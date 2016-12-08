import { Component } from '@angular/core';

import { ICharacter } from '../icharacter.model';
import { MemoryService } from '../memory.service';

@Component({
    selector: 'app-choose-turtle',
    templateUrl: './choose-turtle.component.html',
    styleUrls: [ './choose-turtle.component.css' ]
})
export class ChooseTurtleComponent {
    private _character1: ICharacter = null;
    private _character2: ICharacter = null;

    constructor(private _svc: MemoryService) {

    }

    setCharacter1(character: ICharacter) {
        this._character1 = character;
        this._svc.player1.character = character;
    }

    setCharacter2(character: ICharacter) {
        this._character2 = character;
        this._svc.player2.character = character;
    }

    get readyToPlay(): boolean {
        return this._character1 != null && this._character2 != null;
    }

    play(): void {
        this._svc.startGame();
    }
}
