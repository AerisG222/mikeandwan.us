import { Component, Input, ViewChild, ChangeDetectorRef, AfterViewInit } from '@angular/core';
import { Dialog } from '../../ng_maw/dialog/Dialog';
import { DialogButton } from '../../ng_maw/dialog/DialogButton';
import { ResponsiveService } from '../../ng_maw/services/ResponsiveService';
import { IPhoto } from '../interfaces/IPhoto';

@Component({
    selector: 'photodialog',	
    directives: [ Dialog ],
    templateUrl: '/js/photos/components/PhotoDialog.html'
})
export class PhotoDialog implements AfterViewInit {
    @ViewChild(Dialog) dialog : Dialog;
    @Input() photo : IPhoto = null;
    maxHeight : string = '480px';
    maxWidth : string = '640px';

    constructor(private _responsiveService : ResponsiveService,
                private _changeDetectionRef : ChangeDetectorRef) {
        
    }
    
    ngAfterViewInit() : void {
        this.dialog.buttons = [
            new DialogButton('Close', 'close')
        ];
        
        this._changeDetectionRef.detectChanges();
    }
    
    setMaxDimensions() {
        this.maxHeight = `${this._responsiveService.getHeight() - 200}px`;
        this.maxWidth = `${this._responsiveService.getWidth() - 200}px`;
    }
    
    show() : void {
        this.dialog.show();
    }
    
    hide(evt : any) : void {
        this.dialog.hide();
    }
}
