import { Component, Output, EventEmitter } from '@angular/core';

import { ICharacter } from '../models/icharacter.model';
import { MemoryService } from '../services/memory.service';

@Component({
    selector: 'app-choose-turtle-grid',
    templateUrl: './choose-turtle-grid.component.html',
    styleUrls: [ './choose-turtle-grid.component.scss' ]
})
export class ChooseTurtleGridComponent {
    private selectedCharacter?: ICharacter;
    allCharacters: Array<ICharacter>;

    @Output() selected: EventEmitter<ICharacter> = new EventEmitter<ICharacter>();

    constructor(private svc: MemoryService) {
        this.allCharacters = this.svc.allCharacters;
    }

    selectCharacter(character: ICharacter): void {
        this.selectedCharacter = character;
        this.selected.next(character);
    }

    isSelected(character: ICharacter): boolean {
        if (this.selectedCharacter == null) {
            return false;
        }

        return character.name === this.selectedCharacter.name;
    }
}
