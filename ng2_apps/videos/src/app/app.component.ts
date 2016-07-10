import { Component, ViewChild } from '@angular/core';
import { ROUTER_DIRECTIVES } from '@angular/router';

import { HeaderComponent } from './header/header.component';
import { PreferenceDialogComponent } from './preference-dialog/preference-dialog.component';
import { VideoStateService } from './shared/video-state.service';

@Component({
    moduleId: module.id,
    selector: 'videos-app',
    directives: [ ROUTER_DIRECTIVES, HeaderComponent, PreferenceDialogComponent ],
    templateUrl: 'app.component.html',
    styleUrls: [ 'app.component.css' ]
})
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
