import { Component, Input, ViewChild, ChangeDetectorRef, AfterViewInit } from '@angular/core';

import { DialogComponent } from '../../ng_maw/dialog/dialog.component';
import { DialogButton } from '../../ng_maw/dialog/dialog-button.model';

import { IPhotoInfo } from '../shared/iphoto-info.model';

@Component({
    selector: 'app-save-dialog',
    templateUrl: './save-dialog.component.html',
    styleUrls: [ './save-dialog.component.css' ]
})
export class SaveDialogComponent implements AfterViewInit {
    @ViewChild(DialogComponent) dialog: DialogComponent;
    private _photoInfo: IPhotoInfo = null;
    path: string = null;

    constructor(private _changeDetectionRef: ChangeDetectorRef) {

    }

    ngAfterViewInit(): void {
        this.dialog.title = 'Save Photo';
        this.dialog.buttons = [
            new DialogButton('Close', 'close')
        ];

        this._changeDetectionRef.detectChanges();
    }

    @Input() set photoInfo(value: IPhotoInfo) {
        this._photoInfo = value;
        this.dialog.title = `Resolution: ${this._photoInfo.width} x ${this._photoInfo.height}`;
    }

    get photoInfo(): IPhotoInfo {
        return this._photoInfo;
    }

    show(): void {
        this.dialog.show();
    }

    hide(evt: any): void {
        this.dialog.hide();
    }
}
