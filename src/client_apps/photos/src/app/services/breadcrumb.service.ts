import { Injectable, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';

import { Breadcrumb } from '../models/breadcrumb.model';

@Injectable()
export class BreadcrumbService {
    private breadcrumbs: Array<Breadcrumb> = [];
    breadcrumbEventEmitter: EventEmitter<Array<Breadcrumb>> = new EventEmitter<Array<Breadcrumb>>();

    constructor(private _router: Router) {

    }

    setBreadcrumbs(breadcrumbs: Array<Breadcrumb>): void {
        for (let i = 0; i < breadcrumbs.length; i++) {
            if (this.breadcrumbs.length > i) {
                if (this.breadcrumbs[i].title !== breadcrumbs[i].title) {
                    this.breadcrumbs[i] = breadcrumbs[i];
                }
            } else {
                this.breadcrumbs.push(breadcrumbs[i]);
            }
        }

        while (this.breadcrumbs.length > breadcrumbs.length) {
            this.breadcrumbs.pop();
        }

        this.notifyBreadcrumbChange();
    }

    navigateToBreadcrumb(dest: Breadcrumb): Promise<any> {
        // remove from end of list until we find the selected entry
        while (this.breadcrumbs.length > 0) {
            const lastBreadcrumb = this.breadcrumbs.pop();

            if (lastBreadcrumb === dest) {
                break;
            }
        }

        this.notifyBreadcrumbChange();

        return this._router.navigate(dest.linkParamArray);
    }

    private notifyBreadcrumbChange(): void {
        this.breadcrumbEventEmitter.next(this.breadcrumbs);
    }
}
