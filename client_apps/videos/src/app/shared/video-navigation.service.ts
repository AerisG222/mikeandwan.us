import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

import { BreadcrumbService } from '../../ng_maw/shared/breadcrumb.service';
import { Breadcrumb } from '../../ng_maw/shared/breadcrumb.model';

import { ICategory } from './icategory.model';


@Injectable()
export class VideoNavigationService {
    constructor(private _router: Router,
                private _navService: BreadcrumbService) {

    }

    gotoYearList(): void {
        this.gotoDestination([ '/videos' ], this.getRootBreadcrumbs());
    }

    gotoCategoryList(year: number): void {
        this.gotoDestination([ '/videos', year ], this.getYearListBreadcrumbs(year));
    }

    gotoVideoList(year: number, category: ICategory): void {
        this.gotoDestination([ '/videos', year, category.id ], this.getCategoryListBreadcrumbs(year, category));
    }

    private getRootBreadcrumbs(): Array<Breadcrumb> {
        return [];
    }

    private getYearListBreadcrumbs(year: number): Array<Breadcrumb> {
        let crumbs: Array<Breadcrumb> = [];

        crumbs.push(this.getYearListBreadcrumb(year));

        return crumbs;
    }

    private getCategoryListBreadcrumbs(year: number, category: ICategory): Array<Breadcrumb> {
        let crumbs: Array<Breadcrumb> = [];

        crumbs.push(this.getYearListBreadcrumb(year));
        crumbs.push(this.getCategoryListBreadcrumb(year, category));

        return crumbs;
    }

    private getYearListBreadcrumb(year: number): Breadcrumb {
        return new Breadcrumb(year.toString(), [ '/videos' ]);
    }

    private getCategoryListBreadcrumb(year: number, category: ICategory): Breadcrumb {
        return new Breadcrumb(category.name, [ '/videos', year ]);
    }

    private gotoDestination(linkParamArray: Array<any>, breadcrumbs: Array<Breadcrumb>): void {
        this._router.navigate(linkParamArray).then(
            (data: any) => this._navService.setBreadcrumbs(breadcrumbs)
        );
    }
}
