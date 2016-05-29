import { Injectable } from '@angular/core';
import { RouteData, RouteParams } from '@angular/router-deprecated';
import { ModeRouteInfo } from './ModeRouteInfo';
import { PhotoDataService } from '../services/PhotoDataService';
import { PhotoSource } from './PhotoSource';
import { CategoryPhotoSource } from './CategoryPhotoSource';
import { CommentPhotoSource } from './CommentPhotoSource';
import { RatingPhotoSource } from './RatingPhotoSource';
import { RandomPhotoSource } from './RandomPhotoSource';
import { RouteMode } from './RouteMode';

@Injectable()
export class PhotoSourceFactory {
	constructor(private _dataService : PhotoDataService) {
        
    }
    
	create(routeData : RouteData, routeParams : RouteParams) : PhotoSource {
        let modeInfo = <ModeRouteInfo>routeData.data;
        let type : string = null;
        let order : string = null;
        
        switch(modeInfo.mode) {
            case RouteMode.Category:
                let categoryId = parseInt(routeParams.get(ModeRouteInfo.PARAM_CATEGORY), 10);
                return new CategoryPhotoSource(this._dataService, categoryId);
            case RouteMode.Comment:
                type = routeParams.get(ModeRouteInfo.PARAM_TYPE);
                order = routeParams.get(ModeRouteInfo.PARAM_ORDER);
                return new CommentPhotoSource(this._dataService, type, order);
            case RouteMode.Rating:
                type = routeParams.get(ModeRouteInfo.PARAM_TYPE);
                order = routeParams.get(ModeRouteInfo.PARAM_ORDER);
                return new RatingPhotoSource(this._dataService, type, order);
            case RouteMode.Random:
                return new RandomPhotoSource(this._dataService);
            default:
                throw new RangeError('invalid mode specified');
        }
    }
}
