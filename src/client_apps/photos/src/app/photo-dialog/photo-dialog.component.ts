import { Component, Input } from '@angular/core';

import {NgbActiveModal} from '@ng-bootstrap/ng-bootstrap';

import { ResponsiveService } from '../../ng_maw/shared';

import { IPhoto } from '../shared/iphoto.model';

@Component({
    selector: 'app-photo-dialog',
    templateUrl: './photo-dialog.component.html',
    styleUrls: [ './photo-dialog.component.css' ]
})
export class PhotoDialogComponent {
    @Input() photo: IPhoto = null;
    maxHeight = '480px';
    maxWidth = '640px';

    constructor(private _modal: NgbActiveModal,
                private _responsiveService: ResponsiveService) {

    }

    setMaxDimensions() {
        this.maxHeight = `${this._responsiveService.getHeight() - 200}px`;
        this.maxWidth = `${this._responsiveService.getWidth() - 200}px`;
    }

    close(): void {
        this._modal.dismiss();
    }
}
