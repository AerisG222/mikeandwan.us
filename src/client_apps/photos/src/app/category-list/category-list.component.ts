import { Component, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { state, style, transition, trigger, useAnimation } from '@angular/animations';

import { fadeIn, fadeOut } from 'maw-common';
import { Observable } from 'rxjs';
import { ICategory } from '../models/icategory.model';
import { PhotoDataService } from '../services/photo-data.service';
import { PhotoStateService } from '../services/photo-state.service';
import { PhotoNavigationService } from '../services/photo-navigation.service';
import { ModeRouteInfo } from '../models/mode-route-info.model';
import { Config } from 'protractor';
import { CategoryIndex } from '../models/category-index.model';

@Component({
    selector: 'app-category-list',
    templateUrl: './category-list.component.html',
    styleUrls: [ './category-list.component.css' ],
    animations: [
        trigger('fadeInOut', [
            state('in', style({opacity: 1})),
            transition(':enter', useAnimation(fadeIn)),
            transition(':leave', useAnimation(fadeOut))
        ])
    ]
})
export class CategoryListComponent implements AfterViewInit {
    private _year: number = null;
    categoryList$: Observable<ICategory[]>;
    page = 1;
    cardsPerPage: number;

    constructor(private _changeDetectorRef: ChangeDetectorRef,
                private _dataService: PhotoDataService,
                private _stateService: PhotoStateService,
                private _navService: PhotoNavigationService,
                private _activatedRoute: ActivatedRoute) {
        this.setCardsPerPage(this._stateService.config);

        const pageIndex = Math.trunc(this._stateService.lastCategoryIndex / this._stateService.config.thumbnailsPerPage);

        this.onChangePage(pageIndex + 1);

        this._stateService.configUpdatedEventEmitter.subscribe((config: Config) => {
            this.setCardsPerPage(config);
        });
    }

    ngAfterViewInit() {
        this._activatedRoute.params.subscribe(params => {
            this._year = parseInt(params[ModeRouteInfo.PARAM_YEAR], 10);

            this.categoryList$ = this._dataService
                .getCategoriesForYear(this._year);
        });
    }

    onCategorySelected(categoryAndIndex: CategoryIndex) {
        if (categoryAndIndex !== null) {
            this._stateService.lastCategoryIndex = categoryAndIndex.index;
            this._navService.gotoCategoryPhotoList(categoryAndIndex.category);
        }
    }

    setCardsPerPage(config: Config): void {
        this.onChangePage(1);
        this.cardsPerPage = config.thumbnailsPerPage;
    }

    onChangePage(page: number) {
        if (page >= 1) {
            this.page = page;
        }
    }
}
