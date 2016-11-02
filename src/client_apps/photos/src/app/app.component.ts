import { Component, ViewChild } from '@angular/core';

import { PreferenceDialogComponent } from './preference-dialog/preference-dialog.component';
import { PhotoStateService } from './shared/photo-state.service';
import { PhotoNavigationService } from './shared/photo-navigation.service';

@Component({
    selector: 'app-root',
    templateUrl: 'app.component.html',
    styleUrls: [ 'app.component.css' ]
})
export class AppComponent {
    @ViewChild(PreferenceDialogComponent) private _prefsDialog: PreferenceDialogComponent;

    constructor(private _stateService: PhotoStateService,
                private _navService: PhotoNavigationService) {
        this._stateService.showPreferencesEventEmitter.subscribe(
            (val: any) => this.showPreferencesDialog()
        );
    }

    showPreferencesDialog(): void {
        this._prefsDialog.show();
    }
}