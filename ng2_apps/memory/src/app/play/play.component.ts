import { Component, OnInit } from '@angular/core';
import { NgIf } from '@angular/common';

import { GameBoardComponent } from '../game-board';
import { WinnerScreenComponent } from '../winner-screen';
import { TieScreenComponent } from '../tie-screen';
import { TurtleScoreComponent } from '../turtle-score';
import { IPlayer, MemoryService } from "../";

@Component({
  moduleId: module.id,
  selector: 'app-play',
  directives: [GameBoardComponent, WinnerScreenComponent, TieScreenComponent, TurtleScoreComponent, NgIf],
  templateUrl: 'play.component.html',
  styleUrls: ['play.component.css']
})
export class PlayComponent implements OnInit {
  private _activePlayer: IPlayer = null;
  winningPlayer: IPlayer = null;
  player1: IPlayer;
  player2: IPlayer;
  isGameOver: boolean = false;
  isTie: boolean = false;

  constructor(private _svc: MemoryService) {
    this.player1 = this._svc.player1;
    this.player2 = this._svc.player2;
  }

  ngOnInit(): void {
    this.startNewGame();
  }

  toggleTurn(): void {
    if (this._activePlayer === this.player1) {
      this._activePlayer = this.player2;
    }
    else {
      this._activePlayer = this.player1;
    }

    this.player1.isPlayersTurn = this._activePlayer === this.player1;
    this.player2.isPlayersTurn = this._activePlayer === this.player2;
  }

  endGame(): void {
    this.isGameOver = true;
    let playerNumber: number = 1;

    if (this.player1.score === this.player2.score) {
      this.isTie = true;
      return;
    }
    else if (this.player1.score > this.player2.score) {
      // force the winner to be the current player
      this._activePlayer = this.player1;
    }
    else {
      this._activePlayer = this.player2;
      playerNumber = 2;
    }

    if (this.player1.character.name === this.player2.character.name) {
      // handle case where both players selected same character
      let tmp = {
        character: {
          name: `Player$(playerNumber}`,
          color: this.player1.character.color,
          img: this.player1.character.img
        },
        score: this.player1.score,
        isPlayersTurn: this.player1.isPlayersTurn
      };

      this.winningPlayer = tmp;
    }
    else {
      this.winningPlayer = this._activePlayer;
    }
  }

  onHit(isGameOver: boolean): void {
    this._activePlayer.score += 1;

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
    this._svc.startGame();
  }

  exit(): void {
    this._svc.resetGame();
  }
}
