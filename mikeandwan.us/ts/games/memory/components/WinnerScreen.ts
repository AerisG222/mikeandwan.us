import { Component, Input, Output, EventEmitter } from '@angular/core';
import { IPlayer } from '../interfaces/IPlayer';

@Component({
	selector: 'winner-screen',
	templateUrl: '/js/games/memory/components/WinnerScreen.html'
})
export class WinnerScreen {
	@Input() player : IPlayer;
	@Output() rematch : EventEmitter<any> = new EventEmitter<any>();
	@Output() newgame : EventEmitter<any> = new EventEmitter<any>();
	
	onRematch() : void {
		this.rematch.next(null);
	}
	
	onNewGame() : void {
		this.newgame.next(null);
	}
}
