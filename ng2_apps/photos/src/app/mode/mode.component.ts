import { Observable } from 'rxjs/Observable';
import { Component, OnInit } from '@angular/core';
import { NgFor } from '@angular/common';
import { RouteParams } from '@angular/router-deprecated';

import { Breadcrumb } from '../../../../ng_maw/src/app/shared';

import { PhotoStateService, PhotoNavigationService, ModeRouteInfo, RouteMode } from '../shared';

@Component({
    moduleId: module.id,
    selector: 'app-mode',
    directives: [ NgFor ],
    templateUrl: 'mode.component.html',
    styleUrls: ['mode.component.css']
})
export class ModeComponent implements OnInit {
    private _mode : RouteMode = null;
	  items : Array<Breadcrumb> = [];
    
    constructor(routeParams : RouteParams, 
                private _stateService : PhotoStateService,
                private _navService : PhotoNavigationService) {
        this._mode = this.getMode(routeParams.get('mode'));
                
        // if we make it back to the mode pages, forget the 'last category' we had
        this._stateService.lastCategoryIndex = 0;
    }
    
    ngOnInit() : void {
        switch(this._mode) {
            case null:
                this.setItems(this._navService.getRootDestinations());
                break;
            case RouteMode.Category:
                this.setItems(this._navService.getYearDestinations());
                break;
            case RouteMode.Comment:
                this.setItems(this._navService.getCommentDestinations());
                break;
            case RouteMode.Rating:
                this.setItems(this._navService.getRatingDestinations());
                break;
        }
    }

    selectItem(dest : Breadcrumb) : void {
        this._navService.gotoSpecificMode(dest, this._mode);
    }
    
    private setItems(obs : Observable<Array<Breadcrumb>>) {
        obs.subscribe(items => this.items = items);
    }   
            
    private getMode(mode : string) : RouteMode {
        switch(mode) {
            case null:
                return null;
            case ModeRouteInfo.PARAM_YEAR:
                return RouteMode.Category;
            case ModeRouteInfo.PARAM_COMMENT:
                return RouteMode.Comment;
            case ModeRouteInfo.PARAM_RATING:
                return RouteMode.Rating;
        }
    }   
}
