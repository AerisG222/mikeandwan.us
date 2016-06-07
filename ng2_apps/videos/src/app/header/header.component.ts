import { Component } from '@angular/core';
import { NgFor } from '@angular/common';

import { VideoStateService } from '../shared';
import { BreadcrumbListComponent } from '../../../../ng_maw/src/app/breadcrumb-list';

@Component({
  moduleId: module.id,
  selector: 'app-header',
  directives: [ NgFor, BreadcrumbListComponent ],
  templateUrl: 'header.component.html',
  styleUrls: ['header.component.css']
})
export class HeaderComponent {
    constructor(private _stateService : VideoStateService) {
        
    }
    
    clickConfig() : void {
        this._stateService.showPreferencesDialog();
    };
}
