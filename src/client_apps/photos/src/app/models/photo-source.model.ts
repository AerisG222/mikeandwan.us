import { Observable } from 'rxjs';

import { Photo } from './photo.model';
import { PhotoDataService } from '../services/photo-data.service';

export abstract class PhotoSource {
    constructor(protected _dataService: PhotoDataService) {

    }

    abstract getPhotos(): Observable<Array<Photo>>;
}
