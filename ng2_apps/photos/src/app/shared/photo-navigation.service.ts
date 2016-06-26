import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import { Injectable } from '@angular/core';
import { Router, Instruction } from '@angular/router';

import { BreadcrumbService } from '../../../../ng_maw/src/app/shared/breadcrumb.service';
import { Breadcrumb } from '../../../../ng_maw/src/app/shared/breadcrumb.model';

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
            new Breadcrumb('By Year', this._router.generate(['SpecificMode', { mode: 'year' }])),
            new Breadcrumb('By Comment', this._router.generate(['SpecificMode', { mode: 'comment' }])),
            new Breadcrumb('By Rating', this._router.generate(['SpecificMode', { mode: 'rating' }])),
            new Breadcrumb('Random', this._router.generate(['RandomPhotoList']))
        ];

        return this.getArrayObservable(result);
    }

    getCommentDestinations(): Observable<Array<Breadcrumb>> {
        let result = [
            new Breadcrumb('Newest', this._router.generate(['CommentPhotoList', { type: 'age', order: 'newest' }])),
            new Breadcrumb('Oldest', this._router.generate(['CommentPhotoList', { type: 'age', order: 'oldest' }])),
            new Breadcrumb('Your Newest', this._router.generate(['CommentPhotoList', { type: 'your', order: 'newest' }])),
            new Breadcrumb('Your Oldest', this._router.generate(['CommentPhotoList', { type: 'your', order: 'oldest' }])),
            new Breadcrumb('Most Commented', this._router.generate(['CommentPhotoList', { type: 'qty', order: 'most' }])),
            new Breadcrumb('Least Commented', this._router.generate(['CommentPhotoList', { type: 'qty', order: 'least' }]))
        ];

        return this.getArrayObservable(result);
    }

    getRatingDestinations(): Observable<Array<Breadcrumb>> {
        let result = [
            new Breadcrumb('Average Rating', this._router.generate(['RatingPhotoList', { type: 'avg', order: 'newest' }])),
            new Breadcrumb('Your Rating', this._router.generate(['RatingPhotoList', { type: 'your', order: 'newest' }]))
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
                    let d = result.map(x => new Breadcrumb(x.toString(), this._router.generate(['CategoryList', { year: x }])));
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
                    let d = result.map(x => new CategoryBreadcrumb(x.name, this._router.generate(['CategoryPhotoList', { year: x.year, category: x.id }]), x));
                    observer.next(d);
                    observer.complete();
                });
        });
    }

    gotoModeList(): void {
        let ins = this._router.generate(['Mode']);

        this.gotoDestination(ins, this.getRootBreadcrumbs());
    }

    gotoSpecificMode(dest: Breadcrumb, mode: RouteMode): void {
        this.gotoDestination(dest.routerInstruction, this.getModeBreadcrumbs(dest, mode));
    }

    gotoCategoryPhotoList(dest: CategoryBreadcrumb): void {
        let bcs = this.getCategoryListBreadcrumbs(dest);

        this.gotoDestination(dest.routerInstruction, bcs);
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
            crumbs.push(new Breadcrumb(dest.title, this._router.generate(['SpecificMode', { mode: 'year' }])));
        }
        else if (mode === RouteMode.Comment) {
            crumbs.push(this.getPrimaryModeBreadcrumb('By Comment'));
            crumbs.push(new Breadcrumb(dest.title, this._router.generate(['SpecificMode', { mode: 'comment' }])));
        }
        else if (mode === RouteMode.Rating) {
            crumbs.push(this.getPrimaryModeBreadcrumb('By Rating'));
            crumbs.push(new Breadcrumb(dest.title, this._router.generate(['SpecificMode', { mode: 'rating' }])));
        }

        return crumbs;
    }

    private getCategoryListBreadcrumbs(cb: CategoryBreadcrumb): Array<Breadcrumb> {
        // lets reuse our existing function to start building out the list of breadcrumbs
        let catListBreadcrumb = new Breadcrumb(cb.category.year.toString(), null);
        let crumbs = this.getModeBreadcrumbs(catListBreadcrumb, RouteMode.Category);

        crumbs.push(new CategoryBreadcrumb(cb.category.name,
            this._router.generate(['CategoryList', { year: cb.category.year }]),
            cb.category));

        return crumbs;
    }

    private getPrimaryModeBreadcrumb(title: string) {
        return new Breadcrumb(title, this._router.generate(['Mode']));
    }

    private gotoDestination(routerInstruction: Instruction, breadcrumbs: Array<Breadcrumb>): void {
        this._router.navigateByInstruction(routerInstruction).then(
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
