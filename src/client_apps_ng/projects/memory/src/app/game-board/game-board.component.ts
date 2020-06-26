import { Component, Output, EventEmitter } from '@angular/core';

import { CardComponent } from '../card/card.component';
import { ICardInfo } from '../models/icard-info.model';
import { ISelectedCards } from '../models/iselected-card.model';
import { MemoryService } from '../services/memory.service';

@Component({
    selector: 'app-game-board',
    templateUrl: './game-board.component.html',
    styleUrls: [ './game-board.component.scss' ]
})
export class GameBoardComponent {
    private static CARDS_IN_GAME = 20;

    private selectedCards: ISelectedCards = { card1: null, card2: null };
    private matchedCards: Array<CardComponent> = [];
    private ignoreSelect = false;

    board?: Array<Array<ICardInfo>>;
    @Output() match: EventEmitter<boolean> = new EventEmitter<boolean>();
    @Output() nonmatch: EventEmitter<void> = new EventEmitter<void>();

    constructor(private svc: MemoryService) {
        this.board = this.generateGameBoard();
    }

    removeCards(card1: CardComponent, card2: CardComponent): void {
        this.matchedCards.push(card1);
        this.matchedCards.push(card2);

        setTimeout(() => {
            card1.isRemoved = true;
            card2.isRemoved = true;
        }, 700);

        setTimeout(() => {
            this.prepareNextTurn(true);
        }, 800);
    }

    unFlipCards(): void {
        setTimeout(() => {
            const card1 = this.selectedCards?.card1;
            const card2 = this.selectedCards?.card2;

            if (!!card1) {
                card1.isFlipped = false;
            }

            if (!!card2) {
                card2.isFlipped = false;
            }

            this.prepareNextTurn(false);
        }, 800);
    }

    prepareNextTurn(wasMatch: boolean): void {
        this.selectedCards.card1 = null;
        this.selectedCards.card2 = null;

        this.ignoreSelect = false;
    }

    evaluateTurn(): void {
        const card1 = this.selectedCards?.card1;
        const card2 = this.selectedCards?.card2;

        if (!!card1 && !!card2 && card1.cardInfo?.id === card2.cardInfo?.id) {
            this.removeCards(card1, card2);
            this.match.next(this.matchedCards.length === GameBoardComponent.CARDS_IN_GAME);
        } else {
            this.unFlipCards();
            this.nonmatch.next();
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

        if (this.selectedCards.card1 == null) {
            this.selectedCards.card1 = card;
            this.ignoreSelect = false;
        } else {
            this.selectedCards.card2 = card;
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
            const rand = this.getRandomInt(i, cards.length);
            const tmp = cards[i];
            cards[i] = cards[rand];
            cards[rand] = tmp;
        }
    }

    private generateGameBoard(): Array<Array<ICardInfo>> {
        const sourceCards = this.svc.allCards;
        const gameCards: Array<ICardInfo> = [];

        // get the cards that will be on the board, and add a copy for the matching elements
        for (let i = 0; i < 10; i++) {
            const idxToRemove = this.getRandomInt(0, sourceCards.length);
            const c = sourceCards.splice(idxToRemove, 1)[0];

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
