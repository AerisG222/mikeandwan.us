import { Injectable, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';

import { Breadcrumb } from './breadcrumb.model';

@Injectable()
export class BreadcrumbService {
    private breadcrumbs: Array<Breadcrumb> = [];
    breadcrumbEventEmitter: EventEmitter<Array<Breadcrumb>> = new EventEmitter<Array<Breadcrumb>>();

    constructor(private _router: Router) {

    }

    setBreadcrumbs(breadcrumbs: Array<Breadcrumb>): void {
        this.breadcrumbs = breadcrumbs;
        this.breadcrumbEventEmitter.next(this.breadcrumbs);
    }

    navigateToBreadcrumb(dest: Breadcrumb): Promise<any> {
        // remove from end of list until we find the selected entry
        while (this.breadcrumbs.length > 0) {
            let lastBreadcrumb = this.breadcrumbs.pop();

            if (lastBreadcrumb === dest) {
                break;
            }
        }

        this.notifyBreadcrumbChange();

        return this._router.navigateByInstruction(dest.routerInstruction);
    }

    private notifyBreadcrumbChange(): void {
        this.breadcrumbEventEmitter.next(this.breadcrumbs);
    }
}
