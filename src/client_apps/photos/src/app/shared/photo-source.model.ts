import { Observable } from 'rxjs/Observable';

import { Photo } from './photo.model';
import { PhotoDataService } from './photo-data.service';

export abstract class PhotoSource {
    constructor(protected _dataService: PhotoDataService) {

    }

    abstract getPhotos(): Observable<Array<Photo>>;
}
