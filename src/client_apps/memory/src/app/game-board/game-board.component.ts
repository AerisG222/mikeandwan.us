import { Component, Output, EventEmitter } from '@angular/core';

import { CardComponent } from '../card/card.component';
import { ICardInfo } from '../icard-info.model';
import { ISelectedCards } from '../iselected-card.model';
import { MemoryService } from '../memory.service';

@Component({
    selector: 'app-game-board',
    templateUrl: 'game-board.component.html',
    styleUrls: [ 'game-board.component.css' ]
})
export class GameBoardComponent {
    private static CARDS_IN_GAME: number = 20;
    protected board: Array<Array<ICardInfo>> = null;
    private _selectedCards: ISelectedCards = { card1: null, card2: null };
    private _matchedCards: Array<CardComponent> = [];
    private ignoreSelect: boolean = false;
    @Output() match: EventEmitter<boolean> = new EventEmitter<boolean>();
    @Output() nonmatch: EventEmitter<any> = new EventEmitter<any>();

    constructor(private _svc: MemoryService) {
        this.board = this.generateGameBoard();
    }

    removeCards(): void {
        this._matchedCards.push(this._selectedCards.card1);
        this._matchedCards.push(this._selectedCards.card2);

        setTimeout(() => {
            this._selectedCards.card1.isRemoved = true;
            this._selectedCards.card2.isRemoved = true;
        }, 700);

        setTimeout(() => {
            this.prepareNextTurn(true);
        }, 800);
    }

    unFlipCards(): void {
        setTimeout(() => {
            this._selectedCards.card1.isFlipped = false;
            this._selectedCards.card2.isFlipped = false;

            this.prepareNextTurn(false);
        }, 800);
    }

    prepareNextTurn(wasMatch: boolean): void {
        this._selectedCards.card1 = null;
        this._selectedCards.card2 = null;

        this.ignoreSelect = false;
    }

    evaluateTurn(): void {
        if (this._selectedCards.card1.cardInfo.id === this._selectedCards.card2.cardInfo.id) {
            this.removeCards();
            this.match.next(this._matchedCards.length === GameBoardComponent.CARDS_IN_GAME);
        } else {
            this.unFlipCards();
            this.nonmatch.next(null);
        }
    }

    selectCard(card: CardComponent): void {
        // if a user is clicking on the same card ignore it
        // if the overlay is shown, a user tried click quickly when we are not finished processing the previous click, so ignore it
        if (card.isFlipped || this.ignoreSelect) {
            return;
        }

        this.ignoreSelect = true;
        card.isFlipped = true;

        if (this._selectedCards.card1 == null) {
            this._selectedCards.card1 = card;
            this.ignoreSelect = false;
        } else {
            this._selectedCards.card2 = card;
            this.evaluateTurn();
        }
    }

    // from MDN:
    // Returns a random integer between min (included) and max (excluded)
    // Using Math.round() will give you a non-uniform distribution!
    private getRandomInt(min: number, max: number): number {
        return Math.floor(Math.random() * (max - min)) + min;
    }

    // http://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
    private shuffle(cards: Array<ICardInfo>): void {
        for (let i = 0; i < cards.length; i++) {
            let rand = this.getRandomInt(i, cards.length);
            let tmp = cards[i];
            cards[i] = cards[rand];
            cards[rand] = tmp;
        };
    }

    private generateGameBoard(): Array<Array<ICardInfo>> {
        let sourceCards = this._svc.allCards;
        let gameCards: Array<ICardInfo> = [];

        // get the cards that will be on the board, and add a copy for the matching elements
        for (let i = 0; i < 10; i++) {
            let idxToRemove = this.getRandomInt(0, sourceCards.length);
            let c = sourceCards.splice(idxToRemove, 1)[0];

            // add clones to the board
            gameCards[i] = JSON.parse(JSON.stringify(c));
            gameCards[i + 10] = JSON.parse(JSON.stringify(c));
        }

        // shuffle the array
        this.shuffle(gameCards);

        // now prepare the board in 5x4 matrix
        return [
            gameCards.slice(0, 5),
            gameCards.slice(5, 10),
            gameCards.slice(10, 15),
            gameCards.slice(15, 20)
        ];
    }
}
