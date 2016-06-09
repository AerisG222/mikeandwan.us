import { Component, Input, Output, EventEmitter } from '@angular/core';
import { NgModel } from '@angular/common';

import { Photo, FilterSettings } from '../shared';

@Component({
    moduleId: module.id,
    selector: 'app-effects-view',
    directives: [NgModel],
    templateUrl: 'effects-view.component.html',
    styleUrls: ['effects-view.component.css']
})
export class EffectsViewComponent {
    private _photo: Photo;
    @Output() update: EventEmitter<FilterSettings> = new EventEmitter<FilterSettings>();

    @Input() set photo(value: Photo) {
        this._photo = value;
    }

    get photo(): Photo {
        return this._photo;
    }

    get filters(): FilterSettings {
        return this._photo.filters;
    }

    onChange(): void {
        this.update.next(this._photo.filters);
    }
}
