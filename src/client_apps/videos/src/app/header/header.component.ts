import { Component } from '@angular/core';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

import { SvgIcon } from '../../ng_maw/svg-icon/svg-icon.enum';

import { PreferenceDialogComponent } from '../preference-dialog/preference-dialog.component';

@Component({
    selector: 'app-header',
    templateUrl: './header.component.html',
    styleUrls: [ './header.component.css' ]
})
export class HeaderComponent {
    svgIcon = SvgIcon;

    constructor(private modalService: NgbModal) {

    }

    clickConfig(): void {
        const modalRef = this.modalService.open(PreferenceDialogComponent);
    }
}
