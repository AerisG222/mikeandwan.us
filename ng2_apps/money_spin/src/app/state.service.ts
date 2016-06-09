import { Injectable } from '@angular/core';
import { Router } from '@angular/router-deprecated';

import { Character, Player } from './';

@Injectable()
export class StateService {
    private _winningScore = 100;
    private _splashShown = false;
    player1Characters = [
        new Character('C3P0',  '/img/games/money_spin/p1c3p0.png'),
        new Character('R2D2',  '/img/games/money_spin/p1r2d2.png'),
        new Character('Leo',   '/img/games/money_spin/p1leo.png'),
        new Character('Mikey', '/img/games/money_spin/p1mikey.png')
    ];
    player2Characters = [
        new Character('C3P0',  '/img/games/money_spin/p2c3p0.png'),
        new Character('R2D2',  '/img/games/money_spin/p2r2d2.png'),
        new Character('Leo',   '/img/games/money_spin/p2leo.png'),
        new Character('Mikey', '/img/games/money_spin/p2mikey.png')
    ];
    player1: Player;
    player2: Player;
    currentPlayer: Player;

    constructor(private _router: Router) {

    }

    setPlayer1(character: Character): void {
        this.player1 = new Player(character);
    }

    setPlayer2(character: Character): void {
        this.player2 = new Player(character);
    }

    newGame(): void {
        this.player1 = null;
        this.player2 = null;
        this.currentPlayer = null;
        this._router.navigate(['ChoosePlayer']);
    }

    startGame(): void {
        this.player1.resetScore();
        this.player2.resetScore();

        this.currentPlayer = this.player1;

        this._router.navigate(['Play']);
    }

    evaluateTurn(dollarValue: number): boolean {
        this.currentPlayer.addDollarAmount(dollarValue);

        if (this.currentPlayer.score >= this._winningScore) {
            this._router.navigate(['Winner']);
            return true;
        }

        this.updateCurrentPlayer();

        return false;
    }

    setSplashShown(): void {
        this._splashShown = true;
    }

    private updateCurrentPlayer(): void {
        if (this.currentPlayer == null) {
            this.currentPlayer = this.player1;
        }
        else if (this.currentPlayer === this.player1) {
            this.currentPlayer = this.player2;
        }
        else {
            this.currentPlayer = this.player1;
        }
    }
}
