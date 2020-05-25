import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

import { Character } from '../models/character.model';
import { Player } from '../models/player.model';

@Injectable()
export class StateService {
    private winningScore = 25;
    private splashShown = false;
    player1Characters = [
        new Character('C3P0',  '/js/money_spin/assets/p1c3p0.png'),
        new Character('R2D2',  '/js/money_spin/assets/p1r2d2.png'),
        new Character('Leo',   '/js/money_spin/assets/p1leo.png'),
        new Character('Mikey', '/js/money_spin/assets/p1mikey.png')
    ];
    player2Characters = [
        new Character('C3P0',  '/js/money_spin/assets/p2c3p0.png'),
        new Character('R2D2',  '/js/money_spin/assets/p2r2d2.png'),
        new Character('Leo',   '/js/money_spin/assets/p2leo.png'),
        new Character('Mikey', '/js/money_spin/assets/p2mikey.png')
    ];
    player1: Player;
    player2: Player;
    currentPlayer: Player;

    constructor(private router: Router) {

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
        this.router.navigate(['/choose']);
    }

    isReadyToPlay(): boolean {
        return this.player1 != null &&
               this.player1.character != null &&
               this.player2 != null &&
               this.player2.character != null;
    }

    startGame(): void {
        this.player1.resetScore();
        this.player2.resetScore();

        this.currentPlayer = this.player1;

        this.router.navigate(['/play']);
    }

    evaluateTurn(dollarValue: number): boolean {
        this.currentPlayer.addDollarAmount(dollarValue);

        if (this.currentPlayer.score >= this.winningScore) {
            this.router.navigate(['/winner']);
            return true;
        }

        this.updateCurrentPlayer();

        return false;
    }

    setSplashShown(): void {
        this.splashShown = true;
    }

    private updateCurrentPlayer(): void {
        if (this.currentPlayer == null) {
            this.currentPlayer = this.player1;
        } else if (this.currentPlayer === this.player1) {
            this.currentPlayer = this.player2;
        } else {
            this.currentPlayer = this.player1;
        }
    }
}
