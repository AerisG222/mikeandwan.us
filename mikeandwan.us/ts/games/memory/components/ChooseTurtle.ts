import { Component } from '@angular/core';
import { ChooseTurtleGrid } from './ChooseTurtleGrid';
import { ICharacter } from '../interfaces/ICharacter';
import { MemoryService } from '../services/MemoryService';

@Component({
    selector: 'choose-turtle',
    directives: [ ChooseTurtleGrid ],
    templateUrl: '/js/games/memory/components/ChooseTurtle.html'
})
export class ChooseTurtle {
    private _character1 : ICharacter = null;
    private _character2 : ICharacter = null;
    
    constructor(private _svc : MemoryService) {
        
    }
    
    setCharacter1(character : ICharacter) {
        this._character1 = character;
        this._svc.player1.character = character;
    }

    setCharacter2(character : ICharacter) {
        this._character2 = character;
        this._svc.player2.character = character;
    }
    
    get readyToPlay() : boolean {
        return this._character1 != null && this._character2 != null;
    }
    
    play() : void {
        this._svc.startGame();
    }
}
