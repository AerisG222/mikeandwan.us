import { Injectable } from '@angular/core';
import { Router } from '@angular/router-deprecated';

import { ICardInfo, ICharacter, IPlayer } from './';

@Injectable()
export class MemoryService {
    private _player1: IPlayer = null;
    private _player2: IPlayer = null;

    constructor(private _router: Router) {
        this.initPlayers();
    }

    private _cards: Array<ICardInfo> = [
        { id: 1,  img: '/img/games/memory/cards/card1.jpg' },
        { id: 2,  img: '/img/games/memory/cards/card2.jpg' },
        { id: 3,  img: '/img/games/memory/cards/card3.jpg' },
        { id: 4,  img: '/img/games/memory/cards/card4.jpg' },
        { id: 5,  img: '/img/games/memory/cards/card5.jpg' },
        { id: 6,  img: '/img/games/memory/cards/card6.jpg' },
        { id: 7,  img: '/img/games/memory/cards/card7.jpg' },
        { id: 8,  img: '/img/games/memory/cards/card8.jpg' },
        { id: 9,  img: '/img/games/memory/cards/card9.jpg' },
        { id: 10, img: '/img/games/memory/cards/card10.jpg' },
        { id: 11, img: '/img/games/memory/cards/card11.jpg' }
    ];

    private _characters: Array<ICharacter> = [
        { name: 'Leonardo',      color: 'blue',   img: '/img/games/memory/players/leonardo.jpg' },
        { name: 'Michaelangelo', color: 'orange', img: '/img/games/memory/players/michaelangelo.jpg' },
        { name: 'Donatello',     color: 'purple', img: '/img/games/memory/players/donatello.jpg' },
        { name: 'Raphael',       color: 'red',    img: '/img/games/memory/players/raphael.jpg' }
    ];

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

        this._router.navigateByUrl("/play");
        /*
        if(!this._router.isRouteActive(this._router.generate([ 'Play']))) {
            this._router.navigate([ 'Play' ]);
        }
        */
    }

    resetGame(): void {
        this.initPlayers();
        this._router.navigate(['ChooseTurtle']);
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
