import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import { Injectable } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';

import { BreadcrumbService } from '../../ng_maw/shared/breadcrumb.service';
import { Breadcrumb } from '../../ng_maw/shared/breadcrumb.model';

import { PhotoDataService } from './photo-data.service';
import { CategoryBreadcrumb } from './category-breadcrumb.model';
import { RouteMode } from './route-mode.model';

@Injectable()
export class PhotoNavigationService {
    private _isInitialized = false;

    constructor(private _router: Router,
                private _navService: BreadcrumbService,
                private _dataService: PhotoDataService) {
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
                switch (parts[0]) {
                    case 'year':
                        crumbs.push(new Breadcrumb('By Year', [ '/' ]));

                        if (parts.length > 1) {
                            crumbs.push(new Breadcrumb(parts[1], [ '/year' ]));
                        }

                        if (parts.length > 2) {
                            this.getCategoryDestinations(parseInt(parts[1], 10)).subscribe(x => {
                                let matches = x.filter(y => y.category.id === parseInt(parts[2], 10));

                                if (matches.length === 1) {
                                    crumbs.push(new Breadcrumb(matches[0].title, [ '/year/' + parts[1] ]));
                                    this._navService.setBreadcrumbs(crumbs);
                                }
                            });
                        }

                        break;
                    case 'comment':
                        crumbs.push(new Breadcrumb('By Comment', [ '/' ]));

                        if (parts.length > 2) {
                            this.getCommentDestinations().subscribe(x => {
                                let match = x.filter(y => y.linkParamArray[1] === parts[1] &&
                                                          y.linkParamArray[2] === parts[2])[0];

                                crumbs.push(new Breadcrumb(match.title, [ '/comment' ]));
                            });
                        }

                        break;
                    case 'rating':
                        crumbs.push(new Breadcrumb('By Rating', [ '/' ]));

                        if (parts.length > 2) {
                            this.getRatingDestinations().subscribe(x => {
                                let match = x.filter(y => y.linkParamArray[1] === parts[1] &&
                                                          y.linkParamArray[2] === parts[2])[0];

                                crumbs.push(new Breadcrumb(match.title, [ '/rating' ]));
                            });
                        }

                        break;
                    case 'random':
                        crumbs.push(new Breadcrumb('Random', [ '/' ]));
                        break;
                }
            }

            this._navService.setBreadcrumbs(crumbs);
            this._isInitialized = true;
        }
    }

    getRootDestinations(): Observable<Array<Breadcrumb>> {
        let result = [
            new Breadcrumb('By Year', [ '/year' ]),
            new Breadcrumb('By Comment', [ '/comment' ]),
            new Breadcrumb('By Rating', [ '/rating' ]),
            new Breadcrumb('Random', [ '/random' ])
        ];

        return this.getArrayObservable(result);
    }

    getCommentDestinations(): Observable<Array<Breadcrumb>> {
        let result = [
            new Breadcrumb('Newest', [ '/comment', 'age', 'newest' ]),
            new Breadcrumb('Oldest', [ '/comment', 'age', 'oldest' ]),
            new Breadcrumb('Your Newest', [ '/comment', 'your', 'newest' ]),
            new Breadcrumb('Your Oldest', [ '/comment', 'your', 'oldest' ]),
            new Breadcrumb('Most Commented', [ '/comment', 'qty', 'most' ]),
            new Breadcrumb('Least Commented', [ '/comment', 'qty', 'least' ])
        ];

        return this.getArrayObservable(result);
    }

    getRatingDestinations(): Observable<Array<Breadcrumb>> {
        let result = [
            new Breadcrumb('Average Rating', [ '/rating', 'avg', 'newest' ]),
            new Breadcrumb('Your Rating', [ '/rating', 'your', 'newest' ])
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
                    let d = result.map(x => new Breadcrumb(x.toString(), [ '/year', x ]));
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
                    let d = result.map(x => new CategoryBreadcrumb(x.name, [ '/year', x.year, x.id ], x));
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
        } else if (mode === RouteMode.Category) {
            crumbs.push(this.getPrimaryModeBreadcrumb('By Year'));
            crumbs.push(new Breadcrumb(dest.title, [ '/year' ]));
        } else if (mode === RouteMode.Comment) {
            crumbs.push(this.getPrimaryModeBreadcrumb('By Comment'));
            crumbs.push(new Breadcrumb(dest.title, [ '/comment' ]));
        } else if (mode === RouteMode.Rating) {
            crumbs.push(this.getPrimaryModeBreadcrumb('By Rating'));
            crumbs.push(new Breadcrumb(dest.title, [ '/rating' ]));
        }

        return crumbs;
    }

    private getCategoryListBreadcrumbs(cb: CategoryBreadcrumb): Array<Breadcrumb> {
        // lets reuse our existing function to start building out the list of breadcrumbs
        let catListBreadcrumb = new Breadcrumb(cb.category.year.toString(), null);
        let crumbs = this.getModeBreadcrumbs(catListBreadcrumb, RouteMode.Category);

        crumbs.push(new CategoryBreadcrumb(cb.category.name, [ '/year', cb.category.year ], cb.category));

        return crumbs;
    }

    private getPrimaryModeBreadcrumb(title: string) {
        return new Breadcrumb(title, [ '/' ]);
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
