import { Component, Input } from '@angular/core';

import { IPhoto } from '../shared/iphoto.model';
import { PhotoDataService } from '../shared/photo-data.service';

@Component({
    selector: 'rating-view',
    templateUrl: 'rating-view.component.html',
    styleUrls: [ 'rating-view.component.css' ]
})
export class RatingViewComponent {
    averageRating = -1;
    userRating = -1;
    private _photo: IPhoto;

    @Input() set photo(value: IPhoto) {
        this._photo = value;
        this.getRating();
    }

    get photo(): IPhoto {
        return this._photo;
    }

    constructor(private _dataService: PhotoDataService) {

    }

    getRating(): void {
        if (this._photo != null) {
            this._dataService.getPhotoRatingData(this._photo.id)
                .subscribe(data => {
                    this.averageRating = data.averageRating;
                    this.userRating = data.userRating;
                });
        }
    }

    onRatePhoto(rating: number): void {
        this._dataService.ratePhoto(this._photo.id, rating)
            .subscribe(x => this.getRating());
    }
}
