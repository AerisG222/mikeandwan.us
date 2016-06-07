import { Component, ViewChild, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { NgModel } from '@angular/common';

import { DialogComponent, DialogButton } from '../../../../ng_maw/src/app/dialog';

import { PhotoStateService } from '../shared';

@Component({
    moduleId: module.id,
    selector: 'app-preference-dialog',
    directives: [ DialogComponent, NgModel ],
    templateUrl: 'preference-dialog.component.html',
    styleUrls: ['preference-dialog.component.css']
})
export class PreferenceDialogComponent implements AfterViewInit {
    @ViewChild(DialogComponent) dialog : DialogComponent;
    editDisplayMode : string = null;
    editRowCount : string = null;
    editSlideshowInterval : string = null;
        
    constructor(private _stateService : PhotoStateService,
                private _changeDetectionRef : ChangeDetectorRef) {
        this.updateFormValues();
    }
    
    ngAfterViewInit() : void {
        this.dialog.title = 'Preferences';
        this.dialog.buttons = [
            new DialogButton('Cancel', 'cancel'),
            new DialogButton('Save Preferences', 'save')
        ];
        
        this._changeDetectionRef.detectChanges();
    }
    
    show() : void {
        this.dialog.show();
    }
    
    execute(btn : DialogButton) : void {
        switch(btn.cmd) {
            case 'save':
                this.save();
                break;
            case 'cancel':
                this.cancel();
                break;
        }
    }
    
    save() : void {
        this._stateService.config.displayMode = parseInt(this.editDisplayMode, 10);
        this._stateService.config.slideshowIntervalSeconds = parseInt(this.editSlideshowInterval, 10);
        this._stateService.config.rowsPerPage = parseInt(this.editRowCount, 10);
        this._stateService.saveConfig();
        
        this.cancel();  // leverage this function to perform our cleanup       
    }
    
    updateFormValues() : void {
        this.editDisplayMode = this._stateService.config.displayMode.toString();
        this.editRowCount = this._stateService.config.rowsPerPage.toString();
        this.editSlideshowInterval = this._stateService.config.slideshowIntervalSeconds.toString();
    }
    
    cancel() : void {
        this.dialog.hide();
        this.updateFormValues();
    }
}
