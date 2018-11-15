import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

import { ICardInfo } from '../models/icard-info.model';
import { ICharacter } from '../models/icharacter.model';
import { IPlayer } from '../models/iplayer.model';

@Injectable()
export class MemoryService {
    private _player1: IPlayer = null;
    private _player2: IPlayer = null;

    private _cards: Array<ICardInfo> = [
        { id: 1,  img: '/js/memory/assets/cards/card1.jpg' },
        { id: 2,  img: '/js/memory/assets/cards/card2.jpg' },
        { id: 3,  img: '/js/memory/assets/cards/card3.jpg' },
        { id: 4,  img: '/js/memory/assets/cards/card4.jpg' },
        { id: 5,  img: '/js/memory/assets/cards/card5.jpg' },
        { id: 6,  img: '/js/memory/assets/cards/card6.jpg' },
        { id: 7,  img: '/js/memory/assets/cards/card7.jpg' },
        { id: 8,  img: '/js/memory/assets/cards/card8.jpg' },
        { id: 9,  img: '/js/memory/assets/cards/card9.jpg' },
        { id: 10, img: '/js/memory/assets/cards/card10.jpg' },
        { id: 11, img: '/js/memory/assets/cards/card11.jpg' }
    ];

    private _characters: Array<ICharacter> = [
        { name: 'Leonardo',      color: 'blue',   img: '/js/memory/assets/players/leonardo.jpg' },
        { name: 'Michaelangelo', color: 'orange', img: '/js/memory/assets/players/michaelangelo.jpg' },
        { name: 'Donatello',     color: 'purple', img: '/js/memory/assets/players/donatello.jpg' },
        { name: 'Raphael',       color: 'red',    img: '/js/memory/assets/players/raphael.jpg' }
    ];

    constructor(private _router: Router) {
        this.initPlayers();
    }

    get allCards(): Array<ICardInfo> {
        return this._cards.slice(0); // clone
    }

    get allCharacters(): Array<ICharacter> {
        return this._characters.slice(0); // clone
    }

    get player1(): IPlayer {
        return this._player1;
    }

    set player1(player: IPlayer) {
        this._player1 = player;
    }

    get player2(): IPlayer {
        return this._player2;
    }

    set player2(player: IPlayer) {
        this._player2 = player;
    }

    startGame(): void {
        this.player1.score = 0;
        this.player2.score = 0;

        this._router.navigateByUrl('/play');
    }

    isReadyToPlay(): boolean {
        return this.player1 != null &&
               this.player1.character != null &&
               this.player2 != null &&
               this.player2.character != null;
    }

    resetGame(): void {
        this.initPlayers();
        this._router.navigateByUrl('/');
    }

    initPlayers(): void {
        this._player1 = this.initPlayer();
        this._player2 = this.initPlayer();
    }

    initPlayer(): IPlayer {
        return {
            character: null,
            isPlayersTurn: false,
            score: 0
        };
    }
}
