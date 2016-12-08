import { Component, ViewChild } from '@angular/core';

import { PreferenceDialogComponent } from './preference-dialog/preference-dialog.component';
import { VideoStateService } from './shared/video-state.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: [ './app.component.css' ]
})
export class AppComponent {
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
