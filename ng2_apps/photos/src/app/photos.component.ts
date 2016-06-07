import { Component, ViewChild } from '@angular/core';
import { RouterOutlet, Routes, Route } from '@angular/router';

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
@Routes([
    new Route({ path: '/',                     component: ModeComponent }),
    new Route({ path: '/random',               component: PhotoListComponent, data: new ModeRouteInfo(RouteMode.Random) }),
    new Route({ path: '/year/:year',           component: CategoryListComponent }),
    new Route({ path: '/year/:year/:category', component: PhotoListComponent, data: new ModeRouteInfo(RouteMode.Category) }),
    new Route({ path: '/comment/:type/:order', component: PhotoListComponent, data: new ModeRouteInfo(RouteMode.Comment) }),
    new Route({ path: '/rating/:type/:order',  component: PhotoListComponent, data: new ModeRouteInfo(RouteMode.Rating) }),
    new Route({ path: '/:mode',                component: ModeComponent })
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
