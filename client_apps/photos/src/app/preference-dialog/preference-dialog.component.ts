import { Component, ViewChild, AfterViewInit, ChangeDetectorRef } from '@angular/core';

import { DialogComponent } from '../../ng_maw/dialog/dialog.component';
import { DialogButton } from '../../ng_maw/dialog/dialog-button.model';

import { PhotoStateService } from '../shared/photo-state.service';

@Component({
    selector: 'preference-dialog',
    directives: [ DialogComponent ],
    templateUrl: 'preference-dialog.component.html',
    styleUrls: [ 'preference-dialog.component.css' ]
})
export class PreferenceDialogComponent implements AfterViewInit {
    @ViewChild(DialogComponent) dialog: DialogComponent;
    form = {
        displayMode: null,
        rowCount: null,
        slideshowInterval: null
    };

    constructor(private _stateService: PhotoStateService,
                private _changeDetectionRef: ChangeDetectorRef) {
        this.updateFormValues();
    }

    ngAfterViewInit(): void {
        this.dialog.title = 'Preferences';
        this.dialog.buttons = [
            new DialogButton('Cancel', 'cancel'),
            new DialogButton('Save Preferences', 'save')
        ];

        this._changeDetectionRef.detectChanges();
    }

    show(): void {
        this.dialog.show();
    }

    execute(btn: DialogButton): void {
        switch (btn.cmd) {
            case 'save':
                this.save();
                break;
            case 'cancel':
                this.cancel();
                break;
        }
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
        this.dialog.hide();
        this.updateFormValues();
    }
}
