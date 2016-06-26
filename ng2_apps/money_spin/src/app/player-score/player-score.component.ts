import { Component, Input } from '@angular/core';
import { NgClass } from '@angular/common';

import { Player } from '../player.model';

@Component({
    moduleId: module.id,
    selector: 'app-player-score',
    directives: [ NgClass ],
    templateUrl: 'player-score.component.html',
    styleUrls: [ 'player-score.component.css' ]
})
export class PlayerScoreComponent {
    @Input() player: Player;
    @Input() currentPlayer: Player;
}
