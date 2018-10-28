import { Component, Input, Output, EventEmitter } from '@angular/core';

import { SvgIcon } from '../svg-icon/svg-icon.enum';
import { FilterSettings } from '../models/filter-settings.model';
import { Photo } from '../models/photo.model';

@Component({
    selector: 'app-effects-view',
    templateUrl: './effects-view.component.html',
    styleUrls: [ './effects-view.component.css' ]
})
export class EffectsViewComponent {
    private _photo: Photo;
    svgIcon = SvgIcon;
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
