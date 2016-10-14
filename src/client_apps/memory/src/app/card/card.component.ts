import { Component, Input, Output, EventEmitter } from '@angular/core';

import { ICardInfo } from '../icard-info.model';

@Component({
    selector: 'app-card',
    templateUrl: 'card.component.html',
    styleUrls: [ 'card.component.css' ]
})
export class CardComponent {
    private _isFlipped: boolean = false;
    private _isRemoved: boolean = false;
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

    protected onClick() {
        this.select.next(this);
    }
}
