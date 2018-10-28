import { Component, Input, Output, EventEmitter } from '@angular/core';

import { ICardInfo } from '../models/icard-info.model';

@Component({
    selector: 'app-card',
    templateUrl: './card.component.html',
    styleUrls: [ './card.component.css' ]
})
export class CardComponent {
    private _isFlipped = false;
    private _isRemoved = false;
    @Input() cardInfo: ICardInfo;
    @Output() select: EventEmitter<CardComponent> = new EventEmitter<CardComponent>();

    get isFlipped(): boolean {
        return this._isFlipped;
    }

    set isFlipped(value: boolean) {
        this._isFlipped = value;
    }

    get isRemoved(): boolean {
        return this._isRemoved;
    }

    set isRemoved(value: boolean) {
        this._isRemoved = value;
    }

    onClick() {
        this.select.next(this);
    }
}
