import { Component } from '@angular/core';

import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { VideoStateService } from '../services/video-state.service';

@Component({
    selector: 'app-preference-dialog',
    templateUrl: './preference-dialog.component.html',
    styleUrls: [ './preference-dialog.component.css' ]
})
export class PreferenceDialogComponent {
    orig = { preferLarge: false };
    form = { preferLarge: false };

    constructor(private _modal: NgbActiveModal,
                private _stateService: VideoStateService) {
        this.orig.preferLarge = this._stateService.config.preferFullSize;
        this.form.preferLarge = this._stateService.config.preferFullSize;
    }

    save() {
        if (this.orig.preferLarge !== this.form.preferLarge) {
            this.orig.preferLarge = this.form.preferLarge;
            this._stateService.config.preferFullSize = this.form.preferLarge;
            this._stateService.saveConfig();
        }

        this.cancel();  // leverage this function to perform our cleanup
    }

    cancel() {
        this._modal.dismiss();
        this.form.preferLarge = this.orig.preferLarge;
    }
}
