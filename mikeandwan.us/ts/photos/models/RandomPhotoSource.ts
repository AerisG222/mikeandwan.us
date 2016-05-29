import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import { PhotoDataService } from '../services/PhotoDataService';
import { PhotoSource } from '../models/PhotoSource';
import { Photo } from '../models/Photo';
import { IPhotoAndCategory } from '../interfaces/IPhotoAndCategory';

export class RandomPhotoSource extends PhotoSource {
	constructor(dataService : PhotoDataService) {
		super(dataService);
	}
	
	getPhotos() : Observable<Array<Photo>> {
		return Observable.create((observer : Observer<Array<Photo>>) => {
			this._dataService
				.getRandomPhoto()
				.subscribe((x : IPhotoAndCategory) => {
					let arr : Array<Photo> = [];
					
					arr.push(new Photo(x.photo, x.category));
					
					observer.next(arr);
					observer.complete();
				});
		});
	}
}
