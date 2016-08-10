import { Component, Input } from '@angular/core';

import { Player } from '../player.model';

@Component({
    selector: 'player-score',
    templateUrl: 'player-score.component.html',
    styleUrls: [ 'player-score.component.css' ]
})
export class PlayerScoreComponent {
    @Input() player: Player;
    @Input() currentPlayer: Player;
}
