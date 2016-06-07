import { Component, ViewChild } from '@angular/core';
import { Routes, Route, ROUTER_DIRECTIVES } from '@angular/router';

import { HeaderComponent } from './header';
import { PreferenceDialogComponent } from './preference-dialog';
import { YearListComponent } from './year-list';
import { CategoryListComponent } from './category-list';
import { VideoListComponent } from './video-list';
import { VideoStateService } from './shared';

@Component({
    moduleId: module.id,
    selector: 'videos-app',
    directives: [ ROUTER_DIRECTIVES, HeaderComponent, PreferenceDialogComponent ],
    templateUrl: 'videos.component.html',
    styleUrls: ['videos.component.css']
})
@Routes([
    new Route({ path: '/',                component: YearListComponent }),
    new Route({ path: '/:year',           component: CategoryListComponent }),
    new Route({ path: '/:year/:category', component: VideoListComponent })
])
export class VideosAppComponent {
    @ViewChild(PreferenceDialogComponent) preferenceDialog : PreferenceDialogComponent;
    
    constructor(private _stateService : VideoStateService) {
        this._stateService.showPreferencesEventEmitter.subscribe(
            (val : any) => this.showPreferencesDialog()
        );
    }
    
    showPreferencesDialog() : void {
        this.preferenceDialog.show();
    }
}
