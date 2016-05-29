import { Component, EventEmitter } from '@angular/core';
import { NgFor } from '@angular/common';
import { BreadcrumbService } from '../services/BreadcrumbService';
import { Breadcrumb } from '../services/Breadcrumb';

@Component({
    selector: 'breadcrumbs',
    directives: [ NgFor ],
    templateUrl: '/js/ng_maw/breadcrumbList/BreadcrumbList.html'
})
export class BreadcrumbList {
	breadcrumbs : Array<Breadcrumb> = [];
    breadcrumbsChangedEventEmitter : EventEmitter<Array<Breadcrumb>> = new EventEmitter<Array<Breadcrumb>>();
    
    constructor(private _navService : BreadcrumbService) {
        this._navService.breadcrumbEventEmitter.subscribe(
            (data : Array<Breadcrumb>) => {
                this.breadcrumbs = data;
                this.breadcrumbsChangedEventEmitter.next(data);
            }
        );
    }
    
    clickBreadcrumb(breadcrumb : Breadcrumb) : void {
        this._navService.navigateToBreadcrumb(breadcrumb);
    }
}
