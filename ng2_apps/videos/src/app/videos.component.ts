import { Component, ViewChild } from '@angular/core';
import { RouteConfig, ROUTER_DIRECTIVES } from '@angular/router-deprecated';

import { HeaderComponent } from './header';
import { PreferenceDialogComponent } from './preference-dialog';
import { YearListComponent } from './year-list';
import { CategoryListComponent } from './category-list';
import { VideoListComponent } from './video-list';
import { VideoStateService } from './shared';

@Component({
    moduleId: module.id,
    selector: 'videos-app',
    directives: [ROUTER_DIRECTIVES, HeaderComponent, PreferenceDialogComponent],
    templateUrl: 'videos.component.html',
    styleUrls: ['videos.component.css']
})
@RouteConfig([
    { path: '/',                name: 'YearList',     component: YearListComponent },
    { path: '/:year',           name: 'CategoryList', component: CategoryListComponent },
    { path: '/:year/:category', name: 'VideoList',    component: VideoListComponent }
])
export class VideosAppComponent {
    @ViewChild(PreferenceDialogComponent) preferenceDialog: PreferenceDialogComponent;

    constructor(private _stateService: VideoStateService) {
        this._stateService.showPreferencesEventEmitter.subscribe(
            (val: any) => this.showPreferencesDialog()
        );
    }

    showPreferencesDialog(): void {
        this.preferenceDialog.show();
    }
}
