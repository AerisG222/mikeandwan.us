import { Component, Input, Output, EventEmitter } from '@angular/core';
import { NgClass } from '@angular/common';
import { ICardInfo } from '../interfaces/ICardInfo';

@Component({
    selector: 'card',
    directives: [ NgClass ],
    templateUrl: '/js/games/memory/components/Card.html'
})
export class Card {
    private _isFlipped : boolean = false;
    private _isRemoved : boolean = false;
    @Input() cardInfo : ICardInfo;
    @Output() select : EventEmitter<Card> = new EventEmitter<Card>();
    
    get isFlipped() : boolean {
        return this._isFlipped;
    }
    
    set isFlipped(value : boolean) {
        this._isFlipped = value;
    }
    
    get isRemoved() : boolean {
        return this._isRemoved;
    }
    
    set isRemoved(value : boolean) {
        this._isRemoved = value;
    }
    
    protected onClick() {
        this.select.next(this);
    }
}
