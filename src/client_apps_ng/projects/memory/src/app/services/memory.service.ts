import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

import { ICardInfo } from '../models/icard-info.model';
import { ICharacter } from '../models/icharacter.model';
import { IPlayer } from '../models/iplayer.model';

@Injectable()
export class MemoryService {
    private playerOne: IPlayer = null;
    private playerTwo: IPlayer = null;

    private cards: Array<ICardInfo> = [
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

    private characters: Array<ICharacter> = [
        { name: 'Leonardo',      color: 'blue',   img: '/js/memory/assets/players/leonardo.jpg' },
        { name: 'Michaelangelo', color: 'orange', img: '/js/memory/assets/players/michaelangelo.jpg' },
        { name: 'Donatello',     color: 'purple', img: '/js/memory/assets/players/donatello.jpg' },
        { name: 'Raphael',       color: 'red',    img: '/js/memory/assets/players/raphael.jpg' }
    ];

    constructor(private router: Router) {
        this.initPlayers();
    }

    get allCards(): Array<ICardInfo> {
        return this.cards.slice(0); // clone
    }

    get allCharacters(): Array<ICharacter> {
        return this.characters.slice(0); // clone
    }

    get player1(): IPlayer {
        return this.playerOne;
    }

    set player1(player: IPlayer) {
        this.playerOne = player;
    }

    get player2(): IPlayer {
        return this.playerTwo;
    }

    set player2(player: IPlayer) {
        this.playerTwo = player;
    }

    startGame(): void {
        this.playerOne.score = 0;
        this.playerTwo.score = 0;

        this.router.navigateByUrl('/play');
    }

    isReadyToPlay(): boolean {
        return this.playerOne != null &&
               this.playerOne.character != null &&
               this.playerTwo != null &&
               this.playerTwo.character != null;
    }

    resetGame(): void {
        this.initPlayers();
        this.router.navigateByUrl('/');
    }

    initPlayers(): void {
        this.playerOne = this.initPlayer();
        this.playerTwo = this.initPlayer();
    }

    initPlayer(): IPlayer {
        return {
            character: null,
            isPlayersTurn: false,
            score: 0
        };
    }
}