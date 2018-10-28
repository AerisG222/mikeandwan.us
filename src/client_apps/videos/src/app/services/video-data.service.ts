import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { ICategory } from '../models/icategory.model';
import { IVideo } from '../models/ivideo.model';
import { EnvironmentConfig } from '../models/environment-config';

@Injectable()
export class VideoDataService {
    constructor(private _http: HttpClient,
                private _cfg: EnvironmentConfig) {

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
        return `${this._cfg.apiUrl}/${relativeUrl}`;
    }
}
