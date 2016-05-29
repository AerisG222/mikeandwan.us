import { Component, Input, Output, EventEmitter } from '@angular/core';
import { NgFor, NgClass } from '@angular/common';
import { Character } from '../models/Character';

@Component({
    selector: 'playerselect',	
    directives: [ NgFor, NgClass ],
    templateUrl: '/js/games/money_spin/components/PlayerSelect.html'
})
export class PlayerSelect {
    @Input() characters : Array<Character> = [];
    @Output() characterSelected = new EventEmitter<Character>();
    selected : Character = null;
    
    onSelect(character : Character) : void {
        this.selected = character;
        this.characterSelected.next(character);
    }
}
