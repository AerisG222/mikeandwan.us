import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import 'rxjs/add/operator/zip';

import { PhotoSource, PhotoDataService, Photo, ICategory, IPhoto } from './';

export class CategoryPhotoSource extends PhotoSource {
	constructor(_dataService : PhotoDataService,
	            private _categoryId : number) {
		super(_dataService);
	}
	
	getPhotos() : Observable<Array<Photo>> {
		return Observable.create((observer : Observer<Array<Photo>>) => {
			let catObs = this.getCategoryObservable();
			let photoObs = this.getPhotoObservable();
            
            catObs.zip(photoObs, (c, p) => { 
                    return { category: c, photos: p };
                })
				.subscribe((result : any) => {
					let cat : ICategory = result.category;
					let photos : Array<IPhoto> = result.photos;
					let list = photos.map(x => new Photo(x, cat, null));
					
					observer.next(list);
					observer.complete();
				});
			});
	}
	
	private getCategoryObservable() : Observable<ICategory> {
		return this._dataService.getCategory(this._categoryId);
	}
	
	private getPhotoObservable() : Observable<Array<IPhoto>> {
		return this._dataService.getPhotosByCategory(this._categoryId);
	}
}
