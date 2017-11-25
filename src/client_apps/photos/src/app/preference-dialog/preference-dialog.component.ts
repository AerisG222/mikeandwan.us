import { Component } from '@angular/core';

import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { PhotoStateService } from '../shared/photo-state.service';

@Component({
    selector: 'app-preference-dialog',
    templateUrl: './preference-dialog.component.html',
    styleUrls: [ './preference-dialog.component.css' ]
})
export class PreferenceDialogComponent {
    form = {
        displayMode: null,
        rowCount: null,
        slideshowInterval: null
    };

    constructor(private _modal: NgbActiveModal,
                private _stateService: PhotoStateService) {
        this.updateFormValues();
    }

    save(): void {
        this._stateService.config.displayMode = parseInt(this.form.displayMode, 10);
        this._stateService.config.slideshowIntervalSeconds = parseInt(this.form.slideshowInterval, 10);
        this._stateService.config.rowsPerPage = parseInt(this.form.rowCount, 10);
        this._stateService.saveConfig();

        this.cancel();  // leverage this function to perform our cleanup
    }

    updateFormValues(): void {
        this.form.displayMode = this._stateService.config.displayMode.toString();
        this.form.rowCount = this._stateService.config.rowsPerPage.toString();
        this.form.slideshowInterval = this._stateService.config.slideshowIntervalSeconds.toString();
    }

    cancel(): void {
        this._modal.dismiss();
        this.updateFormValues();
    }
}
