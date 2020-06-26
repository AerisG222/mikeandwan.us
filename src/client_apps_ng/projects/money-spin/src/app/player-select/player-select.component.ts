import { Component, Input, Output, EventEmitter } from '@angular/core';

import { Character } from '../models/character.model';

@Component({
    selector: 'app-player-select',
    templateUrl: './player-select.component.html',
    styleUrls: [ './player-select.component.scss' ]
})
export class PlayerSelectComponent {
    @Input() characters: Array<Character> = [];
    @Output() characterSelected = new EventEmitter<Character>();
    selected?: Character;

    onSelect(character: Character): void {
        this.selected = character;
        this.characterSelected.next(character);
    }
}
