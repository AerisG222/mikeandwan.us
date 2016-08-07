import { Component, ViewChild, ChangeDetectorRef, AfterViewInit } from '@angular/core';

import { DialogComponent } from '../../ng_maw/dialog/dialog.component';
import { DialogButton } from '../../ng_maw/dialog/dialog-button.model';

import { VideoStateService } from '../shared/video-state.service';

@Component({
    selector: 'preference-dialog',
    directives: [ DialogComponent ],
    templateUrl: 'preference-dialog.component.html',
    styleUrls: [ 'preference-dialog.component.css' ]
})
export class PreferenceDialogComponent implements AfterViewInit {
    @ViewChild(DialogComponent) dialog: DialogComponent;
    orig = { preferLarge: false };
    form = { preferLarge: false };
    title = 'Prefs';

    constructor(private _stateService: VideoStateService,
                private _changeDetectionRef: ChangeDetectorRef) {
        this.orig.preferLarge = this._stateService.config.preferFullSize;
        this.form.preferLarge = this._stateService.config.preferFullSize;
    }

    ngAfterViewInit(): void {
        this.dialog.title = 'Preferences';
        this.dialog.buttons = [
            new DialogButton('Cancel', 'cancel'),
            new DialogButton('Save Preferences', 'save')
        ];

        this._changeDetectionRef.detectChanges();
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

    show(): void {
        this.dialog.show();
    }

    hide(): void {
        this.cancel();
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
        this.dialog.hide();
        this.form.preferLarge = this.orig.preferLarge;
    }
}
