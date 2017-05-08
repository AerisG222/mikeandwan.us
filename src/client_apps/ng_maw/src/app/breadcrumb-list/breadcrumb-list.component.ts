import { Component, EventEmitter } from '@angular/core';
import { trigger, state, style, animate, transition } from '@angular/animations';

import { Breadcrumb } from '../shared/breadcrumb.model';
import { BreadcrumbService } from '../shared/breadcrumb.service';
import { SvgIcon } from '../svg-icon/svg-icon.enum';

@Component({
    selector: 'maw-breadcrumb-list',
    templateUrl: './breadcrumb-list.component.html',
    styleUrls: [ './breadcrumb-list.component.css' ],
    animations: [
        trigger('fadeInOut', [
            state('in', style({opacity: 1})),
            transition('void => *', [
                style({opacity: 0}),
                animate(320)
            ]),
            transition('* => void', [
                animate(320, style({opacity: 1}))
            ])
        ])
    ]
})
export class BreadcrumbListComponent {
    svgIcon = SvgIcon;
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
