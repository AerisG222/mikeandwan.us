import { Component, Input, Output, EventEmitter } from '@angular/core';

import { Character } from '../character.model';

@Component({
    selector: 'app-player-select',
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