import { Component, Input } from '@angular/core';
import { PhotoDataService } from '../services/PhotoDataService';
import { Rating } from '../../ng_maw/rating/Rating';
import { IPhoto } from '../interfaces/IPhoto';

@Component({
    selector: 'rating',	
    directives: [ Rating ],
    templateUrl: '/js/photos/components/RatingView.html'
})
export class RatingView {
    averageRating = -1;
    userRating = -1;
    private _photo : IPhoto;
    
    @Input() set photo(value : IPhoto) {
        this._photo = value;
        this.getRating();
    }
    
    get photo() : IPhoto {
        return this._photo;
    }
    
    constructor(private _dataService : PhotoDataService) {
        
    }
    
    getRating() : void {
        if(this._photo != null) {
            this._dataService.getPhotoRatingData(this._photo.id)
                .subscribe(data => {
                    this.averageRating = data.averageRating;
                    this.userRating = data.userRating;
                });    
        }
    }
    
    onRatePhoto(rating : number) : void {
        this._dataService.ratePhoto(this._photo.id, rating)
            .subscribe(x => this.getRating());
    }
}
