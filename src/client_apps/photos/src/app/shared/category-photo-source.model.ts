import { Observable, Observer } from 'rxjs';
import { zip } from 'rxjs/operators';

import { PhotoSource } from './photo-source.model';
import { PhotoDataService } from './photo-data.service';
import { Photo } from './photo.model';
import { ICategory } from './icategory.model';
import { IPhoto } from './iphoto.model';

export class CategoryPhotoSource extends PhotoSource {
    constructor(_dataService: PhotoDataService,
                private _categoryId: number) {
        super(_dataService);
    }

    getPhotos(): Observable<Array<Photo>> {
        return Observable.create((observer: Observer<Array<Photo>>) => {
            const catObs = this.getCategoryObservable();
            const photoObs = this.getPhotoObservable();

            catObs.pipe(
                    zip(photoObs, (c, p) => {
                        return { category: c, photos: p };
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
