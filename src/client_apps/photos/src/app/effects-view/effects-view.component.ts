import { Component, Input, Output, EventEmitter } from '@angular/core';

import { Photo } from '../shared/photo.model';
import { FilterSettings } from '../shared/filter-settings.model';

@Component({
    selector: 'app-effects-view',
    templateUrl: 'effects-view.component.html',
    styleUrls: [ 'effects-view.component.css' ]
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