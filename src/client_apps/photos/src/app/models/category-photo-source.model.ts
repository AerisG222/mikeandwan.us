import { Observable, Observer, zip } from 'rxjs';
import { map } from 'rxjs/operators';

import { PhotoSource } from './photo-source.model';
import { Photo } from './photo.model';
import { ICategory } from './icategory.model';
import { IPhoto } from './iphoto.model';
import { PhotoDataService } from '../services/photo-data.service';

export class CategoryPhotoSource extends PhotoSource {
    constructor(_dataService: PhotoDataService,
                private _categoryId: number) {
        super(_dataService);
    }

    getPhotos(): Observable<Array<Photo>> {
        return Observable.create((observer: Observer<Array<Photo>>) => {
            const catObs = this.getCategoryObservable();
            const photoObs = this.getPhotoObservable();

            zip(catObs, photoObs)
                .pipe(
                    map(r => {
                        return { category: r[0], photos: r[1] };
                    })
                )
                .subscribe((result: any) => {
                    const cat: ICategory = result.category;
                    const photos: Array<IPhoto> = result.photos;
                    const list = photos.map(x => new Photo(x, cat, null));

                    observer.next(list);
                    observer.complete();
                });
        });
    }

    private getCategoryObservable(): Observable<ICategory> {
        return this._dataService.getCategory(this._categoryId);
    }

    private getPhotoObservable(): Observable<Array<IPhoto>> {
        return this._dataService.getPhotosByCategory(this._categoryId);
    }
}