import { Component, ViewChild } from '@angular/core';
import { RouterOutlet, RouteConfig } from '@angular/router-deprecated';
import { Header } from './Header';
import { Mode } from './Mode';
import { CategoryList } from './CategoryList';
import { PhotoList } from './PhotoList';
import { PhotoStateService } from '../services/PhotoStateService';
import { ModeRouteInfo } from '../models/ModeRouteInfo';
import { RouteMode } from '../models/RouteMode';
import { PrefsDialog } from './PrefsDialog';

@Component({
    selector: 'photoapp',	
    directives: [ RouterOutlet, Header, PrefsDialog ],
    templateUrl: '/js/photos/components/App.html'
})
@RouteConfig([
    { path: '/',                     name: 'Mode',              component: Mode },
    { path: '/random',               name: 'RandomPhotoList',   component: PhotoList, data: new ModeRouteInfo(RouteMode.Random) },
    { path: '/year/:year',           name: 'CategoryList',      component: CategoryList },
    { path: '/year/:year/:category', name: 'CategoryPhotoList', component: PhotoList, data: new ModeRouteInfo(RouteMode.Category) },
    { path: '/comment/:type/:order', name: 'CommentPhotoList',  component: PhotoList, data: new ModeRouteInfo(RouteMode.Comment) },
    { path: '/rating/:type/:order',  name: 'RatingPhotoList',   component: PhotoList, data: new ModeRouteInfo(RouteMode.Rating) },
    { path: '/:mode',                name: 'SpecificMode',      component: Mode }
])
// TODO: add otherwise route config item
export class MawPhotoApp {
    @ViewChild(PrefsDialog) private _prefsDialog : PrefsDialog;
    
    constructor(private _stateService : PhotoStateService) {
        this._stateService.showPreferencesEventEmitter.subscribe(
            (val : any) => this.showPreferencesDialog()
        );
    }
    
    showPreferencesDialog() : void {
        this._prefsDialog.show();
    }
}
