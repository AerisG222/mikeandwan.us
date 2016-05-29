import { Component } from '@angular/core';
import { NgFor } from '@angular/common';
import { VideoStateService } from '../services/VideoStateService';
import { BreadcrumbList } from '../../ng_maw/breadcrumbList/BreadcrumbList';

@Component({
    selector: 'header',	
    directives: [ NgFor, BreadcrumbList ],
    templateUrl: '/js/videos/components/Header.html'
})
export class Header {
    constructor(private _stateService : VideoStateService) {
        
    }
    
    clickConfig() : void {
        this._stateService.showPreferencesDialog();
    };
}
