import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';

import { ICategory, IVideo } from './index';

@Injectable()
// TODO: determine if we should cache responses like prior version
export class VideoDataService {
	constructor(private http: Http) {
		
	}
	
	getYears() : Observable<Array<number>> {
		return this.http
			.get('/api/videos/getYears')
			.map((res : Response) => res.json());
	}
	
	getCategoriesForYear(year : number) : Observable<Array<ICategory>> {
		return this.http
			.get(`/api/videos/getCategoriesForYear/${year}`)
			.map((res : Response) => res.json());
	}
	
	getVideosForCategory(categoryId : number) : Observable<Array<IVideo>> {
		return this.http
			.get(`/api/videos/getVideosByCategory/${categoryId}`)
			.map((res : Response) => res.json());
	}
}
