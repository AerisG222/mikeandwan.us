import { Component, Output, EventEmitter } from '@angular/core';
import { NgFor, NgClass } from '@angular/common';

import { ICharacter, MemoryService } from '../';

@Component({
    moduleId: module.id,
    selector: 'app-choose-turtle-grid',
    directives: [ NgFor, NgClass ],
    templateUrl: 'choose-turtle-grid.component.html',
    styleUrls: ['choose-turtle-grid.component.css']
})
export class ChooseTurtleGridComponent {
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
