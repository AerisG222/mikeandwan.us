import { Component, Output, EventEmitter } from '@angular/core';
import { NgFor, NgClass } from '@angular/common';
import { ICharacter } from '../interfaces/ICharacter';
import { MemoryService } from '../services/MemoryService';

@Component({
    selector: 'choose-turtle-grid',
    directives: [ NgFor, NgClass ],
    templateUrl: '/js/games/memory/components/ChooseTurtleGrid.html'
})
export class ChooseTurtleGrid {
    private _selectedCharacter : ICharacter = null;
    allCharacters : Array<ICharacter>;
    @Output() selected : EventEmitter<ICharacter> = new EventEmitter<ICharacter>();
    
    constructor(private _svc : MemoryService) {
        this.allCharacters = this._svc.allCharacters; 
    }
    
    selectCharacter(character : ICharacter) : void {
        this._selectedCharacter = character;
        this.selected.next(character);
    }
    
    isSelected(character : ICharacter) : boolean {
        if(this._selectedCharacter == null) {
            return false;
        }
        
        return character.name === this._selectedCharacter.name;
    }
}
