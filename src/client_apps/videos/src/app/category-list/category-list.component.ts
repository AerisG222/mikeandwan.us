import { Component, ViewChild, ChangeDetectorRef, AfterViewInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { trigger, state, style, transition, useAnimation } from '@angular/animations';

import { NgbPagination } from '@ng-bootstrap/ng-bootstrap';

import { fadeIn, fadeOut } from '../../ng_maw/shared/animation';

import { ICategory } from '../shared/icategory.model';
import { VideoNavigationService } from '../shared/video-navigation.service';
import { SizeService } from '../shared/size.service';
import { VideoDataService } from '../shared/video-data.service';
import { VideoStateService } from '../shared/video-state.service';
import { Config } from '../shared/config.model';

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
    year = -1;
    page = 1;
    categoryList: Array<ICategory> = [];
    cardsPerPage = 24;

    constructor(private _sizeService: SizeService,
                private _dataService: VideoDataService,
                private _stateService: VideoStateService,
                private _navService: VideoNavigationService,
                private _changeDetectionRef: ChangeDetectorRef,
                private _activatedRoute: ActivatedRoute) {

    }

    ngAfterViewInit(): void {
        this._activatedRoute.params.subscribe(params => {
            this.year = parseInt(params['year'], 10);

            this._dataService.getCategoriesForYear(this.year)
                .subscribe(
                    (categories: Array<ICategory>) => {
                        this.categoryList = categories;
                    },
                    (err: any) => console.error(`there was an error: ${err}`)
                );

            this._changeDetectionRef.detectChanges();
        });
    }

    onChangePage(page: number): void {
        if (page >= 1) {
            this.page = page;
        }
    }

    onCategorySelected(category: ICategory): void {
        if (category !== null) {
            this._navService.gotoVideoList(category.year, category);
        }
    }
}
