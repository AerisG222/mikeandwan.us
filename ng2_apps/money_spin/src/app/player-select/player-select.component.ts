import { Component, Input, Output, EventEmitter } from '@angular/core';
import { NgFor, NgClass } from '@angular/common';

import { Character } from '../character.model';

@Component({
    moduleId: module.id,
    selector: 'player-select',
    directives: [ NgFor, NgClass ],
    templateUrl: 'player-select.component.html',
    styleUrls: [ 'player-select.component.css' ]
})
export class PlayerSelectComponent {
    @Input() characters: Array<Character> = [];
    @Output() characterSelected = new EventEmitter<Character>();
    selected: Character = null;

    onSelect(character: Character): void {
        this.selected = character;
        this.characterSelected.next(character);
    }
}
