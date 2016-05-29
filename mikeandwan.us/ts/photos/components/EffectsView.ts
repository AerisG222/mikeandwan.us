import { Component, Input, Output, EventEmitter } from '@angular/core';
import { NgModel } from '@angular/common';
import { Photo } from '../models/Photo';
import { FilterSettings } from '../models/FilterSettings';

@Component({
    selector: 'effects',
    directives: [ NgModel ],
    templateUrl: '/js/photos/components/EffectsView.html'
})
export class EffectsView {
    private _photo : Photo;
    @Output() update : EventEmitter<FilterSettings> = new EventEmitter<FilterSettings>();
    
	@Input() set photo(value: Photo) {
        this._photo = value;
    }

    get photo() : Photo {
        return this._photo;
    }
    
    get filters() : FilterSettings {
        return this._photo.filters;
    }
    
    onChange() : void {
        this.update.next(this._photo.filters);
    }
}
