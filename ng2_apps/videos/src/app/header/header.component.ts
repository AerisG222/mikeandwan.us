import { Component } from '@angular/core';
import { NgFor } from '@angular/common';

import { BreadcrumbListComponent } from '../../../../ng_maw/src/app/breadcrumb-list/breadcrumb-list.component';

import { VideoStateService } from '../shared/video-state.service';

@Component({
    moduleId: module.id,
    selector: 'header',
    directives: [ NgFor, BreadcrumbListComponent ],
    templateUrl: 'header.component.html',
    styleUrls: [ 'header.component.css' ]
})
export class HeaderComponent {
    constructor(private _stateService: VideoStateService) {

    }

    clickConfig(): void {
        this._stateService.showPreferencesDialog();
    };
}
