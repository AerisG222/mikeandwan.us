import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { Photo, PhotoDataService } from './';

@Injectable()
export abstract class PhotoSource {
	constructor(protected _dataService : PhotoDataService) {
		
	}
	
	abstract getPhotos() : Observable<Array<Photo>>;
}
