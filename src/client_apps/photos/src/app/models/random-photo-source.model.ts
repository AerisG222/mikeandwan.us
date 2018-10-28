import { Observable, Observer } from 'rxjs';

import { PhotoSource } from './photo-source.model';
import { Photo } from './photo.model';
import { IPhotoAndCategory } from './iphoto-and-category.model';
import { PhotoDataService } from '../services/photo-data.service';

export class RandomPhotoSource extends PhotoSource {
    constructor(dataService: PhotoDataService) {
        super(dataService);
    }

    getPhotos(): Observable<Array<Photo>> {
        return Observable.create((observer: Observer<Array<Photo>>) => {
            this._dataService
                .getRandomPhoto()
                .subscribe((x: IPhotoAndCategory) => {
                    const arr: Array<Photo> = [];

                    arr.push(new Photo(x.photo, x.category));

                    observer.next(arr);
                    observer.complete();
                });
        });
    }
}
