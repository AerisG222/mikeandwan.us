import { Observable } from 'rxjs/Observable';
import { Component, OnInit } from '@angular/core';
import { NgFor } from '@angular/common';
import { RouteParams } from '@angular/router-deprecated';
import { PhotoStateService } from '../services/PhotoStateService';
import { PhotoNavigationService } from '../services/PhotoNavigationService';
import { Breadcrumb } from '../../ng_maw/services/Breadcrumb';
import { ModeRouteInfo } from '../models/ModeRouteInfo';
import { RouteMode } from '../models/RouteMode';

@Component({
    selector: 'mode',	
    directives: [ NgFor ],
    templateUrl: '/js/photos/components/Mode.html'
})
export class Mode implements OnInit {
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
