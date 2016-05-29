import { Component, Input, Output, EventEmitter } from '@angular/core';
import { IPlayer } from '../interfaces/IPlayer';

@Component({
	selector: 'tie-screen',
	templateUrl: '/js/games/memory/components/TieScreen.html'
})
export class TieScreen {
	@Input() player1 : IPlayer;
	@Input() player2 : IPlayer;
	@Output() rematch : EventEmitter<any> = new EventEmitter<any>();
	@Output() newgame : EventEmitter<any> = new EventEmitter<any>();
	
	onRematch() : void {
		this.rematch.next(null);
	}
	
	onNewGame() : void {
		this.newgame.next(null);
	}
}
