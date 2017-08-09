import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import 'rxjs/add/operator/map';

import { ICategory } from './icategory.model';
import { IPhoto } from './iphoto.model';
import { IExifDetail } from './iexif-detail.model';
import { IRating } from './irating.model';
import { IComment } from './icomment.model';
import { IPhotoAndCategory } from './iphoto-and-category.model';

@Injectable()
export class PhotoDataService {
    constructor(private _http: HttpClient) {

    }

    getRandomPhoto(): Observable<IPhotoAndCategory> {
        return this._http
            .get<IPhotoAndCategory>('/api/photos/getRandomPhoto');
    }

    getYears(): Observable<Array<number>> {
        return this._http
            .get<Array<number>>('/api/photos/getPhotoYears');
    }

    getCategory(categoryId: number): Observable<ICategory> {
        return this._http
            .get<ICategory>(`/api/photos/getCategory/${categoryId}`);
    }

    getCategoriesForYear(year: number): Observable<Array<ICategory>> {
        return this._http
            .get<Array<ICategory>>(`/api/photos/getCategoriesForYear/${year}`);
    }

    getPhotosByCategory(categoryId: number): Observable<Array<IPhoto>> {
        return this._http
            .get<Array<IPhoto>>(`/api/photos/getPhotosByCategory/${categoryId}`);
    }

    getPhotosByCommentDate(newestFirst: boolean): Observable<Array<IPhotoAndCategory>> {
        return this._http
            .get<Array<IPhotoAndCategory>>(`/api/photos/getPhotosAndCategoriesByCommentDate/${newestFirst}`);
    }

    getPhotosByUserCommentDate(newestFirst: boolean): Observable<Array<IPhotoAndCategory>> {
        return this._http
            .get<Array<IPhotoAndCategory>>(`/api/photos/getPhotosAndCategoriesByUserCommentDate/${newestFirst}`);
    }

    getPhotosByCommentCount(greatestFirst: boolean): Observable<Array<IPhotoAndCategory>> {
        return this._http
            .get<Array<IPhotoAndCategory>>(`/api/photos/getPhotosAndCategoriesByCommentCount/${greatestFirst}`);
    }

    getPhotosByAverageRating(highestFirst: boolean): Observable<Array<IPhotoAndCategory>> {
        return this._http
            .get<Array<IPhotoAndCategory>>(`/api/photos/getPhotosAndCategoriesByAverageRating/${highestFirst}`);
    }

    getPhotosByUserRating(highestFirst: boolean): Observable<Array<IPhotoAndCategory>> {
        return this._http
            .get<Array<IPhotoAndCategory>>(`/api/photos/getPhotosAndCategoriesByUserRating/${highestFirst}`);
    }

    getPhotoExifData(photoId: number): Observable<IExifDetail> {
        return this._http
            .get<IExifDetail>(`/api/photos/getPhotoExifData/${photoId}`);
    }

    getPhotoRatingData(photoId: number): Observable<IRating> {
        return this._http
            .get<IRating>(`/api/photos/getRatingForPhoto/${photoId}`);
    }

    ratePhoto(photoId: number, rating: number): Observable<number> {
        return this._http
            .post<number>('/api/photos/ratePhoto', { photoId: photoId, rating: rating });
    }

    getCommentsForPhoto(photoId: number): Observable<Array<IComment>> {
        return Observable.create((observer: Observer<Array<IComment>>) => {
            this._http
                .get<Array<IComment>>(`/api/photos/getCommentsForPhoto/${photoId}`)
                .subscribe(comments => {
                    // deal with dates
                    const c = comments.map((x: IComment) => {
                        x.entryDate = new Date(x.entryDate.toString());
                        return x;
                    });

                    observer.next(c);
                    observer.complete();
                });
        });
    }

    addCommentForPhoto(photoId: number, comment: string): Observable<any> {
        return this._http
            .post('/api/photos/addCommentForPhoto', { photoId: photoId, comment: comment });
    }
}
