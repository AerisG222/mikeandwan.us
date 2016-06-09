import { Component, EventEmitter } from '@angular/core';
import { NgFor } from '@angular/common';

import { BreadcrumbService, Breadcrumb } from '../shared';

@Component({
    moduleId: module.id,
    selector: 'app-breadcrumb-list',
    directives: [NgFor],
    templateUrl: 'breadcrumb-list.component.html',
    styleUrls: ['breadcrumb-list.component.css']
})
export class BreadcrumbListComponent {
    breadcrumbs: Array<Breadcrumb> = [];
    breadcrumbsChangedEventEmitter: EventEmitter<Array<Breadcrumb>> = new EventEmitter<Array<Breadcrumb>>();

    constructor(private _navService: BreadcrumbService) {
        this._navService.breadcrumbEventEmitter.subscribe(
            (data: Array<Breadcrumb>) => {
                this.breadcrumbs = data;
                this.breadcrumbsChangedEventEmitter.next(data);
            }
        );
    }

    clickBreadcrumb(breadcrumb: Breadcrumb): void {
        this._navService.navigateToBreadcrumb(breadcrumb);
    }
}
