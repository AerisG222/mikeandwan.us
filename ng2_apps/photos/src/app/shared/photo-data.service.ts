import { Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';

import { ICategory, IPhoto, IExifDetail, IRating, IComment, IPhotoAndCategory } from './';

@Injectable()
export class PhotoDataService {
    constructor(private _http: Http) {

    }

    getRandomPhoto(): Observable<IPhotoAndCategory> {
        return this._http
            .get('/api/photos/getRandomPhoto')
            .map((res: Response) => res.json());
    }

    getYears(): Observable<Array<number>> {
        return this._http
            .get('/api/photos/getPhotoYears')
            .map((res: Response) => res.json());
    }

    getCategory(categoryId: number): Observable<ICategory> {
        return this._http
            .get('/api/photos/getCategory/' + categoryId)
            .map((res: Response) => res.json());
    }

    getCategoriesForYear(year: number): Observable<Array<ICategory>> {
        return this._http
            .get('/api/photos/getCategoriesForYear/' + year)
            .map((res: Response) => res.json());
    }

    getPhotosByCategory(categoryId: number): Observable<Array<IPhoto>> {
        return this._http
            .get('/api/photos/getPhotosByCategory/' + categoryId)
            .map((res: Response) => res.json());
    }

    getPhotosByCommentDate(newestFirst: boolean): Observable<Array<IPhotoAndCategory>> {
        return this._http
            .get('/api/photos/getPhotosAndCategoriesByCommentDate/' + newestFirst)
            .map((res: Response) => res.json());
    }

    getPhotosByUserCommentDate(newestFirst: boolean): Observable<Array<IPhotoAndCategory>> {
        return this._http
            .get('/api/photos/getPhotosAndCategoriesByUserCommentDate/' + newestFirst)
            .map((res: Response) => res.json());
    }

    getPhotosByCommentCount(greatestFirst: boolean): Observable<Array<IPhotoAndCategory>> {
        return this._http
            .get('/api/photos/getPhotosAndCategoriesByCommentCount/' + greatestFirst)
            .map((res: Response) => res.json());
    }

    getPhotosByAverageRating(highestFirst: boolean): Observable<Array<IPhotoAndCategory>> {
        return this._http
            .get('/api/photos/getPhotosAndCategoriesByAverageRating/' + highestFirst)
            .map((res: Response) => res.json());
    }

    getPhotosByUserRating(highestFirst: boolean): Observable<Array<IPhotoAndCategory>> {
        return this._http
            .get('/api/photos/getPhotosAndCategoriesByUserRating/' + highestFirst)
            .map((res: Response) => res.json());
    }

    getPhotoExifData(photoId: number): Observable<IExifDetail> {
        return this._http
            .get('/api/photos/getPhotoExifData/' + photoId)
            .map((res: Response) => res.json());
    }

    getPhotoRatingData(photoId: number): Observable<IRating> {
        return this._http
            .get('/api/photos/getRatingForPhoto/' + photoId)
            .map((res: Response) => res.json());
    }

    ratePhoto(photoId: number, rating: number): Observable<number> {
        return this._http
            .post('/api/photos/ratePhoto', JSON.stringify({ photoId: photoId, rating: rating }), { headers: this.getPostHeaders() })
            .map((res: Response) => res.json());
    }

    getCommentsForPhoto(photoId: number): Observable<Array<IComment>> {
        return Observable.create((observer: Observer<Array<IComment>>) => {
            this._http
                .get('/api/photos/getCommentsForPhoto/' + photoId)
                .map((res: Response) => res.json())
                .subscribe(comments => {
                    // deal with dates
                    let c = comments.map((x: IComment) => {
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
            .post('/api/photos/addCommentForPhoto', JSON.stringify({ photoId: photoId, comment: comment }), { headers: this.getPostHeaders() })
            .map((res: Response) => res.json());
    }

    private getPostHeaders(): Headers {
        let h = new Headers();
        h.append('Accept', 'application/json');
        h.append('Content-Type', 'application/json');

        return h;
    }
}
