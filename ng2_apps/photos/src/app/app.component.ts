import { Component, ViewChild } from '@angular/core';
import { ROUTER_DIRECTIVES } from '@angular/router';

import { HeaderComponent } from './header/header.component';
import { PreferenceDialogComponent } from './preference-dialog/preference-dialog.component';
import { PhotoStateService } from './shared/photo-state.service';

@Component({
    moduleId: module.id,
    selector: 'photos-app',
    directives: [ ROUTER_DIRECTIVES, HeaderComponent, PreferenceDialogComponent ],
    templateUrl: 'app.component.html',
    styleUrls: [ 'app.component.css' ]
})
export class PhotosAppComponent {
    @ViewChild(PreferenceDialogComponent) private _prefsDialog: PreferenceDialogComponent;

    constructor(private _stateService: PhotoStateService) {
        this._stateService.showPreferencesEventEmitter.subscribe(
            (val: any) => this.showPreferencesDialog()
        );
    }

    showPreferencesDialog(): void {
        this._prefsDialog.show();
    }
}
