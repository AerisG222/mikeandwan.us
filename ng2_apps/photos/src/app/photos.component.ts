import { Component, ViewChild } from '@angular/core';
import { RouterOutlet, RouteConfig } from '@angular/router-deprecated';

import { HeaderComponent } from './header';
import { ModeComponent } from './mode';
import { CategoryListComponent } from './category-list';
import { PhotoListComponent } from './photo-list';
import { PreferenceDialogComponent } from './preference-dialog';
import { PhotoStateService, ModeRouteInfo, RouteMode } from './shared';

@Component({
  moduleId: module.id,
  selector: 'photos-app',
  directives: [ RouterOutlet, HeaderComponent, PreferenceDialogComponent ],
  templateUrl: 'photos.component.html',
  styleUrls: ['photos.component.css']
})
@RouteConfig([
    { path: '/',                     name: 'Mode',         component: ModeComponent },
    { path: '/random',               name: 'PhotoList',    component: PhotoListComponent, data: new ModeRouteInfo(RouteMode.Random) },
    { path: '/year/:year',           name: 'CategoryList', component: CategoryListComponent },
    { path: '/year/:year/:category', name: 'PhotoList',    component: PhotoListComponent, data: new ModeRouteInfo(RouteMode.Category) },
    { path: '/comment/:type/:order', name: 'PhotoList',    component: PhotoListComponent, data: new ModeRouteInfo(RouteMode.Comment) },
    { path: '/rating/:type/:order',  name: 'PhotoList',    component: PhotoListComponent, data: new ModeRouteInfo(RouteMode.Rating) },
    { path: '/:mode',                name: 'Mode',         component: ModeComponent }
])
// TODO: add otherwise route config item
export class PhotosAppComponent {
    @ViewChild(PreferenceDialogComponent) private _prefsDialog : PreferenceDialogComponent;
    
    constructor(private _stateService : PhotoStateService) {
        this._stateService.showPreferencesEventEmitter.subscribe(
            (val : any) => this.showPreferencesDialog()
        );
    }
    
    showPreferencesDialog() : void {
        this._prefsDialog.show();
    }
}
