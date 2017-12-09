import { Component, EventEmitter } from '@angular/core';
import { trigger, query, state, style, animate, stagger, transition, useAnimation } from '@angular/animations';

import { fadeIn, fadeOut } from '../shared/animation';
import { Breadcrumb } from '../shared/breadcrumb.model';
import { BreadcrumbService } from '../shared/breadcrumb.service';
import { SvgIcon } from '../svg-icon/svg-icon.enum';

@Component({
    selector: 'app-breadcrumb-list',
    templateUrl: './breadcrumb-list.component.html',
    styleUrls: [ './breadcrumb-list.component.css' ],
    animations: [
        trigger('fadeInOut', [
            transition('* => *', [
                query(':enter', style({ opacity: 0 }), { optional: true }),
                query(':enter', stagger('100ms', useAnimation(fadeIn)), { optional: true }),
                query(':leave', stagger('25ms', useAnimation(fadeOut)), { optional: true })
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
