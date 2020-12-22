import { Component, Input, Output, EventEmitter } from '@angular/core';

import { ICardInfo } from '../models/icard-info.model';

@Component({
    selector: 'app-card',
    templateUrl: './card.component.html',
    styleUrls: [ './card.component.scss' ]
})
export class CardComponent {
    @Input() cardInfo?: ICardInfo;
    @Output() cardSelected: EventEmitter<CardComponent> = new EventEmitter<CardComponent>();

    private cardIsFlipped = false;
    private cardIsRemoved = false;

    get isFlipped(): boolean {
        return this.cardIsFlipped;
    }

    set isFlipped(value: boolean) {
        this.cardIsFlipped = value;
    }

    get isRemoved(): boolean {
        return this.cardIsRemoved;
    }

    set isRemoved(value: boolean) {
        this.cardIsRemoved = value;
    }

    onClick(): void {
        this.cardSelected.next(this);
    }
}
