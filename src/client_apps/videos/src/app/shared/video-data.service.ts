import { Observable } from 'rxjs/Observable';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { ICategory } from './icategory.model';
import { IVideo } from './ivideo.model';

@Injectable()
export class VideoDataService {
    constructor(private _http: HttpClient) {

    }

    getYears(): Observable<Array<number>> {
        return this._http
            .get<Array<number>>('/api/videos/getYears');
    }

    getCategoriesForYear(year: number): Observable<Array<ICategory>> {
        return this._http
            .get<Array<ICategory>>(`/api/videos/getCategoriesForYear/${year}`);
    }

    getVideosForCategory(categoryId: number): Observable<Array<IVideo>> {
        return this._http
            .get<Array<IVideo>>(`/api/videos/getVideosByCategory/${categoryId}`);
    }
}
