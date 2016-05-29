import { Component, Input } from '@angular/core';
import { NgClass } from '@angular/common';
import { IPlayer } from '../interfaces/IPlayer';

@Component({
	selector: 'turtle-score',
	directives: [ NgClass ],
	templateUrl: '/js/games/memory/components/TurtleScore.html'
})
export class TurtleScore {
	@Input() player : IPlayer;
}
