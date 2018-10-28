import { Component, Input } from '@angular/core';

import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { IPhotoInfo } from '../models/iphoto-info.model';

@Component({
    selector: 'app-save-dialog',
    templateUrl: './save-dialog.component.html',
    styleUrls: [ './save-dialog.component.css' ]
})
export class SaveDialogComponent {
    title: string = null;
    path: string = null;

    @Input() set photoInfo(photo: IPhotoInfo) {
        this.path = photo.path;
        this.title = `Resolution: ${photo.width} x ${photo.height}`;
    }

    constructor(private _modal: NgbActiveModal) {

    }

    close(): void {
        this._modal.close();
    }
}
