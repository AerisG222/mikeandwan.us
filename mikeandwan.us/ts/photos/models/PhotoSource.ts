import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { PhotoDataService } from '../services/PhotoDataService';
import { Photo } from '../models/Photo';

@Injectable()
export abstract class PhotoSource {
	constructor(protected _dataService : PhotoDataService) {
		
	}
	
	abstract getPhotos() : Observable<Array<Photo>>;
}
