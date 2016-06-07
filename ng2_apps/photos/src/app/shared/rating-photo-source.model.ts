import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';

import { PhotoSource, PhotoDataService, Photo, IPhotoAndCategory } from './';

export class RatingPhotoSource extends PhotoSource {
	constructor(_dataService : PhotoDataService,
	            private _type : string,
				private _order : string) {
		super(_dataService);
	}
	
	getPhotos() : Observable<Array<Photo>> {
		return Observable.create((observer : Observer<Array<Photo>>) => {
			let ord : boolean = null;
			let obs : Observable<Array<IPhotoAndCategory>> = null;
			
			switch(this._type) {
				case 'avg':
					ord = this._order === 'newest' ? true : false;
					obs = this._dataService.getPhotosByAverageRating(ord);
					break;
				case 'your':
					ord = this._order === 'newest' ? true : false;
					obs = this._dataService.getPhotosByUserRating(ord);
					break;
				default:
					throw new RangeError('invalid type specified');
			}
		
			obs.subscribe((results : Array<IPhotoAndCategory>) => {
				let result = results.map(x => new Photo(x.photo, x.category));
				
				observer.next(result);
				observer.complete();
			});
		});
	}
}
