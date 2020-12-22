import { Component, OnInit } from '@angular/core';

import { IPlayer } from '../models/iplayer.model';
import { MemoryService } from '../services/memory.service';

@Component({
    selector: 'app-play',
    templateUrl: './play.component.html',
    styleUrls: [ './play.component.scss' ]
})
export class PlayComponent implements OnInit {
    winningPlayer?: IPlayer;
    player1: IPlayer;
    player2: IPlayer;
    isGameOver = false;
    isTie = false;

    private activePlayer?: IPlayer;

    constructor(private svc: MemoryService) {
        if (!!!this.svc.player1) {
            throw new Error('player1 cannot be null');
        }

        if (!!!this.svc.player2) {
            throw new Error('player2 cannot be null');
        }

        this.player1 = this.svc.player1;
        this.player2 = this.svc.player2;
    }

    ngOnInit(): void {
        this.startNewGame();
    }

    toggleTurn(): void {
        if (this.activePlayer === this.player1) {
            this.activePlayer = this.player2;
        } else {
            this.activePlayer = this.player1;
        }

        this.player1.isPlayersTurn = this.activePlayer === this.player1;
        this.player2.isPlayersTurn = this.activePlayer === this.player2;
    }

    endGame(): void {
        this.isGameOver = true;
        let playerNumber = 1;

        if (this.player1.score === this.player2.score) {
            this.isTie = true;
            return;
        } else if (this.player1.score > this.player2.score) {
            // force the winner to be the current player
            this.activePlayer = this.player1;
        } else {
            this.activePlayer = this.player2;
            playerNumber = 2;
        }

        if (this.player1.character.name === this.player2.character.name) {
            // handle case where both players selected same character
            const tmp = {
                character: {
                    name: `Player$(playerNumber}`,
                    color: this.player1.character.color,
                    img: this.player1.character.img
                },
                score: this.player1.score,
                isPlayersTurn: this.player1.isPlayersTurn
            };

            this.winningPlayer = tmp;
        } else {
            this.winningPlayer = this.activePlayer;
        }
    }

    onHit(isGameOver: boolean): void {
        if (!!this.activePlayer) {
            this.activePlayer.score += 1;
        }

        if (isGameOver) {
            this.endGame();
        }
    }

    onMiss(): void {
        this.toggleTurn();
    }

    startNewGame(): void {
        this.isGameOver = false;
        this.isTie = false;
        this.toggleTurn();
        this.svc.startGame();
    }

    exit(): void {
        this.svc.resetGame();
    }
}
