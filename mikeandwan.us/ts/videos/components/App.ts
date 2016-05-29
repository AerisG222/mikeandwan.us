import { Component, ViewChild } from '@angular/core';
import { RouterOutlet, RouteConfig } from '@angular/router-deprecated';
import { Header } from './Header';
import { PrefsDialog } from './PrefsDialog';
import { YearList } from './YearList';
import { CategoryList } from './CategoryList';
import { VideoList } from './VideoList';
import { VideoStateService } from '../services/VideoStateService';

@Component({
    selector: 'videoapp',	
    directives: [ RouterOutlet, Header, PrefsDialog ],
    templateUrl: '/js/videos/components/App.html'
})
@RouteConfig([
    { path: '/',                name: 'YearList',     component: YearList },
    { path: '/:year',           name: 'CategoryList', component: CategoryList },
    { path: '/:year/:category', name: 'VideoList',    component: VideoList }
])
// TODO: add otherwise route config item
export class MawVideoApp {
    @ViewChild(PrefsDialog) dialogPrefs : PrefsDialog;
    
    constructor(private _stateService : VideoStateService) {
        this._stateService.showPreferencesEventEmitter.subscribe(
            (val : any) => this.showPreferencesDialog()
        );
    }
    
    showPreferencesDialog() : void {
        this.dialogPrefs.show();
    }
}
