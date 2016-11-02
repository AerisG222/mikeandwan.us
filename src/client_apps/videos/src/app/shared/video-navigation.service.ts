import { Injectable } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';

import { VideoDataService } from './video-data.service';
import { BreadcrumbService } from '../../ng_maw/shared/breadcrumb.service';
import { Breadcrumb } from '../../ng_maw/shared/breadcrumb.model';

import { ICategory } from './icategory.model';


@Injectable()
export class VideoNavigationService {
    private _isInitialized = false;

    constructor(private _router: Router,
                private _dataService: VideoDataService,
                private _navService: BreadcrumbService) {
        this._router.events.subscribe(evt => {
            if (evt instanceof NavigationEnd) {
                this.onRouterEvent(<NavigationEnd>evt);
            }
        });
    }

    onRouterEvent(navEnd: NavigationEnd): void {
        if (!this._isInitialized) {
            let snapshot = this._router.routerState.snapshot;
            let parts = snapshot.url.toLowerCase().split('/').filter(el => el.length !== 0);
            let crumbs = [];

            if (parts.length > 0) {
                crumbs.push(new Breadcrumb(parts[0], [ '/' ]));

                if (parts.length > 1) {
                    this._dataService.getCategoriesForYear(parseInt(parts[0], 10)).subscribe(x => {
                        let matches = x.filter(y => y.id === parseInt(parts[1], 10));

                        if (matches.length === 1) {
                            crumbs.push(new Breadcrumb(matches[0].name, [ '/' + parts[0] ]));
                            this._navService.setBreadcrumbs(crumbs);
                        }
                    });
                }
            }

            this._navService.setBreadcrumbs(crumbs);
            this._isInitialized = true;
        }
    }

    gotoYearList(): void {
        this.gotoDestination([ '/' ], this.getRootBreadcrumbs());
    }

    gotoCategoryList(year: number): void {
        this.gotoDestination([ '/', year ], this.getYearListBreadcrumbs(year));
    }

    gotoVideoList(year: number, category: ICategory): void {
        this.gotoDestination([ '/', year, category.id ], this.getCategoryListBreadcrumbs(year, category));
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
        return new Breadcrumb(year.toString(), [ '/' ]);
    }

    private getCategoryListBreadcrumb(year: number, category: ICategory): Breadcrumb {
        return new Breadcrumb(category.name, [ '/', year ]);
    }

    private gotoDestination(linkParamArray: Array<any>, breadcrumbs: Array<Breadcrumb>): void {
        this._router.navigate(linkParamArray).then(
            (data: any) => this._navService.setBreadcrumbs(breadcrumbs)
        );
    }
}