import { Component, Input} from '@angular/core';
import { NgClass } from '@angular/common';
import { Player } from '../models/Player';

@Component({
    selector: 'playerscore',
    directives: [ NgClass ],
    templateUrl: '/js/games/money_spin/components/PlayerScore.html'
})
export class PlayerScore {
    @Input() player : Player;
    @Input() currentPlayer : Player;
}
