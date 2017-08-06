import { Observable } from 'rxjs/Observable';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { Breadcrumb } from '../../ng_maw/shared/breadcrumb.model';

import { PhotoStateService } from '../shared/photo-state.service';
import { PhotoNavigationService } from '../shared/photo-navigation.service';
import { ModeRouteInfo } from '../shared/mode-route-info.model';
import { RouteMode } from '../shared/route-mode.model';

@Component({
    selector: 'app-mode',
    templateUrl: './mode.component.html',
    styleUrls: [ './mode.component.css' ]
})
export class ModeComponent implements OnInit {
    private _mode: RouteMode = null;
    items: Array<Breadcrumb> = [];

    constructor(private _stateService: PhotoStateService,
                private _navService: PhotoNavigationService,
                private _activatedRoute: ActivatedRoute) {
        // if we make it back to the mode pages, forget the 'last category' we had
        this._stateService.lastCategoryIndex = 0;
    }

    ngOnInit(): void {
        this._activatedRoute.params.subscribe(params => {
            this._mode = this.getMode(params['mode']);

            switch (this._mode) {
                case undefined:
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
        });
    }

    selectItem(dest: Breadcrumb): void {
        this._navService.gotoSpecificMode(dest, this._mode);
    }

    private setItems(obs: Observable<Array<Breadcrumb>>) {
        obs.subscribe(items => this.items = items);
    }

    private getMode(mode: string): RouteMode {
        switch (mode) {
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
