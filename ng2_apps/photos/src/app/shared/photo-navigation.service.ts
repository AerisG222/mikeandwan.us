import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

import { BreadcrumbService } from '../../ng_maw/shared/breadcrumb.service';
import { Breadcrumb } from '../../ng_maw/shared/breadcrumb.model';

import { PhotoDataService } from './photo-data.service';
import { CategoryBreadcrumb } from './category-breadcrumb.model';
import { RouteMode } from './route-mode.model';

@Injectable()
export class PhotoNavigationService {
    constructor(private _router: Router,
        private _navService: BreadcrumbService,
        private _dataService: PhotoDataService) {

    }

    getRootDestinations(): Observable<Array<Breadcrumb>> {
        let result = [
            new Breadcrumb('By Year', [ '/photos/year' ]),
            new Breadcrumb('By Comment', [ '/photos/comment' ]),
            new Breadcrumb('By Rating', [ '/photos/rating' ]),
            new Breadcrumb('Random', [ '/photos/random' ])
        ];

        return this.getArrayObservable(result);
    }

    getCommentDestinations(): Observable<Array<Breadcrumb>> {
        let result = [
            new Breadcrumb('Newest', [ '/photos/comment', 'age', 'newest' ]),
            new Breadcrumb('Oldest', [ '/photos/comment', 'age', 'oldest' ]),
            new Breadcrumb('Your Newest', [ '/photos/comment', 'your', 'newest' ]),
            new Breadcrumb('Your Oldest', [ '/photos/comment', 'your', 'oldest' ]),
            new Breadcrumb('Most Commented', [ '/photos/comment', 'qty', 'most' ]),
            new Breadcrumb('Least Commented', [ '/photos/comment', 'qty', 'least' ])
        ];

        return this.getArrayObservable(result);
    }

    getRatingDestinations(): Observable<Array<Breadcrumb>> {
        let result = [
            new Breadcrumb('Average Rating', [ '/photos/rating', 'avg', 'newest' ]),
            new Breadcrumb('Your Rating', [ '/photos/rating', 'your', 'newest' ])
            // TODO:
            //   - age : most recent / oldest
            //   - qty : most rated / least rated
        ];

        return this.getArrayObservable(result);
    }

    getYearDestinations(): Observable<Array<Breadcrumb>> {
        return Observable.create((observer: Observer<Array<Breadcrumb>>) => {
            this._dataService
                .getYears()
                .subscribe(result => {
                    let d = result.map(x => new Breadcrumb(x.toString(), [ '/photos/year', x ]));
                    observer.next(d);
                    observer.complete();
                });
        });
    }

    getCategoryDestinations(year: number): Observable<Array<CategoryBreadcrumb>> {
        return Observable.create((observer: Observer<Array<CategoryBreadcrumb>>) => {
            this._dataService
                .getCategoriesForYear(year)
                .subscribe(result => {
                    let d = result.map(x => new CategoryBreadcrumb(x.name, [ '/photos/year', x.year, x.id ], x));
                    observer.next(d);
                    observer.complete();
                });
        });
    }

    gotoModeList(): void {
        this.gotoDestination([ '/' ], this.getRootBreadcrumbs());
    }

    gotoSpecificMode(dest: Breadcrumb, mode: RouteMode): void {
        this.gotoDestination(dest.linkParamArray, this.getModeBreadcrumbs(dest, mode));
    }

    gotoCategoryPhotoList(dest: CategoryBreadcrumb): void {
        let bcs = this.getCategoryListBreadcrumbs(dest);

        this.gotoDestination(dest.linkParamArray, bcs);
    }

    private getRootBreadcrumbs(): Array<Breadcrumb> {
        return [];
    }

    private getModeBreadcrumbs(dest: Breadcrumb, mode: RouteMode): Array<Breadcrumb> {
        let crumbs: Array<Breadcrumb> = [];

        if (mode == null) {
            crumbs.push(this.getPrimaryModeBreadcrumb(dest.title));
        }
        else if (mode === RouteMode.Category) {
            crumbs.push(this.getPrimaryModeBreadcrumb('By Year'));
            crumbs.push(new Breadcrumb(dest.title, [ '/photos/year' ]));
        }
        else if (mode === RouteMode.Comment) {
            crumbs.push(this.getPrimaryModeBreadcrumb('By Comment'));
            crumbs.push(new Breadcrumb(dest.title, [ '/photos/comment' ]));
        }
        else if (mode === RouteMode.Rating) {
            crumbs.push(this.getPrimaryModeBreadcrumb('By Rating'));
            crumbs.push(new Breadcrumb(dest.title, [ '/photos/rating' ]));
        }

        return crumbs;
    }

    private getCategoryListBreadcrumbs(cb: CategoryBreadcrumb): Array<Breadcrumb> {
        // lets reuse our existing function to start building out the list of breadcrumbs
        let catListBreadcrumb = new Breadcrumb(cb.category.year.toString(), null);
        let crumbs = this.getModeBreadcrumbs(catListBreadcrumb, RouteMode.Category);

        crumbs.push(new CategoryBreadcrumb(cb.category.name, [ '/photos/year', cb.category.year ], cb.category));

        return crumbs;
    }

    private getPrimaryModeBreadcrumb(title: string) {
        return new Breadcrumb(title, [ '/photos' ]);
    }

    private gotoDestination(linkParamArray: Array<any>, breadcrumbs: Array<Breadcrumb>): void {
        this._router.navigate(linkParamArray).then(
            (data: any) => this._navService.setBreadcrumbs(breadcrumbs)
        );
    }

    private getArrayObservable(arr: Array<Breadcrumb>): Observable<Array<Breadcrumb>> {
        return Observable.create((observer: Observer<Array<Breadcrumb>>) => {
            observer.next(arr);
            observer.complete();
        });
    }
}
