import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';

import { PhotoSource, PhotoDataService, Photo, IPhotoAndCategory } from './';

export class CommentPhotoSource extends PhotoSource {
    constructor(_dataService: PhotoDataService,
        private _type: string,
        private _order: string) {
        super(_dataService);
    }

    getPhotos(): Observable<Array<Photo>> {
        return Observable.create((observer: Observer<Array<IPhotoAndCategory>>) => {
            let ord: boolean = null;
            let obs: Observable<Array<IPhotoAndCategory>> = null;

            switch (this._type) {
                case 'age':
                    ord = this._order === 'newest' ? true : false;
                    obs = this._dataService.getPhotosByCommentDate(ord);
                    break;
                case 'qty':
                    ord = this._order === 'most' ? true : false;
                    obs = this._dataService.getPhotosByCommentCount(ord);
                    break;
                case 'your':
                    ord = this._order === 'newest' ? true : false;
                    obs = this._dataService.getPhotosByUserCommentDate(ord);
                    break;
                default:
                    throw new RangeError('invalid type specified');
            }

            obs.subscribe((results: Array<IPhotoAndCategory>) => {
                let result = results.map(x => new Photo(x.photo, x.category));

                observer.next(result);
                observer.complete();
            });
        });
    }
}
