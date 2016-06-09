import { Component, Input, ViewChild, ChangeDetectorRef, AfterViewInit } from '@angular/core';

import { DialogComponent, DialogButton } from '../../../../ng_maw/src/app/dialog';
import { ResponsiveService } from '../../../../ng_maw/src/app/shared';

import { IPhoto } from '../shared';

@Component({
    moduleId: module.id,
    selector: 'app-photo-dialog',
    directives: [DialogComponent],
    templateUrl: 'photo-dialog.component.html',
    styleUrls: ['photo-dialog.component.css']
})
export class PhotoDialogComponent implements AfterViewInit {
    @ViewChild(DialogComponent) dialog: DialogComponent;
    @Input() photo: IPhoto = null;
    maxHeight: string = '480px';
    maxWidth: string = '640px';

    constructor(private _responsiveService: ResponsiveService,
        private _changeDetectionRef: ChangeDetectorRef) {

    }

    ngAfterViewInit(): void {
        this.dialog.buttons = [
            new DialogButton('Close', 'close')
        ];

        this._changeDetectionRef.detectChanges();
    }

    setMaxDimensions() {
        this.maxHeight = `${this._responsiveService.getHeight() - 200}px`;
        this.maxWidth = `${this._responsiveService.getWidth() - 200}px`;
    }

    show(): void {
        this.dialog.show();
    }

    hide(evt: any): void {
        this.dialog.hide();
    }
}
