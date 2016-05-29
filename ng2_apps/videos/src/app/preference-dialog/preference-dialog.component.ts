import { Component, ViewChild, ChangeDetectorRef, AfterViewInit } from '@angular/core';
import { NgModel, FORM_DIRECTIVES } from '@angular/common';
import { VideoStateService } from '../shared';
import { DialogComponent, DialogButton } from '../../../../ng_maw/src/app/dialog';

@Component({
    moduleId: module.id,
    selector: 'app-preference-dialog',
    directives: [ DialogComponent, NgModel, FORM_DIRECTIVES ],
    templateUrl: 'preference-dialog.component.html',
    styleUrls: ['preference-dialog.component.css']
})
export class PreferenceDialogComponent implements AfterViewInit {
    @ViewChild(DialogComponent) dialog : DialogComponent;
	  preferLarge : boolean = false;
    form : any = null;
    title : string = "Prefs";
    
    constructor(private _stateService : VideoStateService,
                private _changeDetectionRef : ChangeDetectorRef) {
        this.preferLarge = this._stateService.config.preferFullSize;
        this.form = { preferLarge: this.preferLarge };
    }
    
    ngAfterViewInit() : void {
        this.dialog.title = 'Preferences';
        this.dialog.buttons = [
            new DialogButton('Cancel', 'cancel'),
            new DialogButton('Save Preferences', 'save')
        ];
        
        this._changeDetectionRef.detectChanges();
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
    
    show() : void {
        this.dialog.show();
    }
    
    hide() : void {
        this.cancel();
    }
    
    save() {
        if(this.preferLarge !== this.form.preferLarge)
        {
            this.preferLarge = this.form.preferLarge;
            this._stateService.config.preferFullSize = this.form.preferLarge;
            this._stateService.saveConfig();
        }
        
        this.cancel();  // leverage this function to perform our cleanup       
    }
    
    cancel() {
        this.dialog.hide();
        this.form.preferLarge = this.preferLarge;
    }
}
