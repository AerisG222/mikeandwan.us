import { Component, Input, ViewChild, ChangeDetectorRef, AfterViewInit } from '@angular/core';
import { Dialog } from '../../ng_maw/dialog/Dialog';
import { DialogButton } from '../../ng_maw/dialog/DialogButton';
import { IPhotoInfo } from '../interfaces/IPhotoInfo';

@Component({
    selector: 'savedialog',	
    directives: [ Dialog ],
    templateUrl: '/js/photos/components/SaveDialog.html'
})
export class SaveDialog implements AfterViewInit {
    @ViewChild(Dialog) dialog : Dialog;
    private _photoInfo : IPhotoInfo = null;
    path : string = null;

    constructor(private _changeDetectionRef : ChangeDetectorRef) {
        
    }
    
    ngAfterViewInit() : void {
        this.dialog.title = 'Save Photo';
        this.dialog.buttons = [
            new DialogButton('Close', 'close')
        ];
        
        this._changeDetectionRef.detectChanges();
    }   
    
    @Input() set photoInfo(value : IPhotoInfo) {
        this._photoInfo = value;
        this.dialog.title = `Resolution: ${this._photoInfo.width} x ${this._photoInfo.height}`;
    }
    
    get photoInfo() : IPhotoInfo {
        return this._photoInfo;
    }
    
    show() : void {
        this.dialog.show();
    }
    
    hide(evt : any) : void {
        this.dialog.hide();
    }
}
