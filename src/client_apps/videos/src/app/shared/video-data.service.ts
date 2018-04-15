import { Observable } from 'rxjs/Observable';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { ICategory } from './icategory.model';
import { IVideo } from './ivideo.model';
import { environment } from '../../environments/environment';

@Injectable()
export class VideoDataService {
    constructor(private _http: HttpClient) {

    }

    getYears(): Observable<Array<number>> {
        const url = this.getAbsoluteUrl('videos/getYears');

        return this._http
            .get<Array<number>>(url);
    }

    getCategoriesForYear(year: number): Observable<Array<ICategory>> {
        const url = this.getAbsoluteUrl(`videos/getCategoriesForYear/${year}`);

        return this._http
            .get<Array<ICategory>>(url);
    }

    getVideosForCategory(categoryId: number): Observable<Array<IVideo>> {
        const url = this.getAbsoluteUrl(`videos/getVideosByCategory/${categoryId}`);

        return this._http
            .get<Array<IVideo>>(url);
    }


    getAbsoluteUrl(relativeUrl: string) {
        return `${environment.apiUrl}/${relativeUrl}`;
    }
}
