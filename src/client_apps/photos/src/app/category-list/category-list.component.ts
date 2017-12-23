import { Component, ViewChild, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { animate, state, style, transition, trigger, useAnimation } from '@angular/animations';

import { NgbPagination } from '@ng-bootstrap/ng-bootstrap';

import { fadeIn, fadeOut } from '../shared/animation';
import { SvgIcon } from '../svg-icon/svg-icon.enum';
import { Config, ModeRouteInfo, PhotoNavigationService, PhotoStateService, ICategory, PhotoDataService } from '../shared';

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
    categoryList: Array<ICategory> = [];
    page = 1;
    cardsPerPage: number;

    constructor(private _changeDetectorRef: ChangeDetectorRef,
                private _dataService: PhotoDataService,
                private _stateService: PhotoStateService,
                private _navService: PhotoNavigationService,
                private _activatedRoute: ActivatedRoute) {

    }

    ngAfterViewInit() {
        this._activatedRoute.params.subscribe(params => {
            this._year = parseInt(params[ModeRouteInfo.PARAM_YEAR], 10);
            this.setCardsPerPage(this._stateService.config);

            this._stateService.configUpdatedEventEmitter.subscribe((config: Config) => {
                this.setCardsPerPage(config);
            });

            this._dataService.getCategoriesForYear(this._year)
                .subscribe(categories => {
                    this.categoryList = categories;
                    /*
                    const lastIndex = this._stateService.lastCategoryIndex;

                    if (lastIndex > 0) {
                        const page = Math.floor(lastIndex / this.thumbnailList.itemsPerPage);

                        this.thumbnailList.setPageDisplayedIndex(page);
                    }
                    */
                });

            this._changeDetectorRef.detectChanges();
        });
    }

    onChangePage(page: number) {
        if (page >= 1) {
            this.page = page;
        }
    }

    onCategorySelected(category: ICategory) {
        if (category !== null) {
            // this._stateService.lastCategoryIndex = thumbInfo.index;
            this._navService.gotoCategoryPhotoList(category);
        }
    }

    setCardsPerPage(config: Config): void {
        this.page = 1;
        this.cardsPerPage = config.thumbnailsPerPage;
    }
}
