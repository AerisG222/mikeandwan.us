import { Injectable } from '@angular/core';
import { Data, Params } from '@angular/router';

import { ModeRouteInfo } from './mode-route-info.model';
import { PhotoDataService } from './photo-data.service';
import { PhotoSource } from './photo-source.model';
import { CategoryPhotoSource } from './category-photo-source.model';
import { CommentPhotoSource } from './comment-photo-source.model';
import { RatingPhotoSource } from './rating-photo-source.model';
import { RandomPhotoSource } from './random-photo-source.model';
import { RouteMode } from './route-mode.model';

@Injectable()
export class PhotoSourceFactory {
    constructor(private _dataService: PhotoDataService) {

    }

    create(routeData: Data, routeParams: Params): PhotoSource {
        const modeInfo = routeData[ModeRouteInfo.PARAM_MODE];
        let type: string = null;
        let order: string = null;

        switch (modeInfo) {
            case RouteMode.Category:
                const categoryId = parseInt(routeParams[ModeRouteInfo.PARAM_CATEGORY], 10);
                return new CategoryPhotoSource(this._dataService, categoryId);
            case RouteMode.Comment:
                type = routeParams[ModeRouteInfo.PARAM_TYPE];
                order = routeParams[ModeRouteInfo.PARAM_ORDER];
                return new CommentPhotoSource(this._dataService, type, order);
            case RouteMode.Rating:
                type = routeParams[ModeRouteInfo.PARAM_TYPE];
                order = routeParams[ModeRouteInfo.PARAM_ORDER];
                return new RatingPhotoSource(this._dataService, type, order);
            case RouteMode.Random:
                return new RandomPhotoSource(this._dataService);
            default:
                throw new RangeError('invalid mode specified');
        }
    }
}
