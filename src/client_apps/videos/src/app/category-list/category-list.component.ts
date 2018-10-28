import { Component, ChangeDetectorRef, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { trigger, state, style, transition, useAnimation } from '@angular/animations';

import { fadeIn, fadeOut } from '../animations/animation';
import { ICategory } from '../models/icategory.model';
import { Observable } from 'rxjs';
import { VideoDataService } from '../services/video-data.service';
import { VideoNavigationService } from '../services/video-navigation.service';

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
export class CategoryListComponent implements OnInit {
    year = -1;
    page = 1;
    categoryList$: Observable<ICategory[]>;
    cardsPerPage = 24;

    constructor(private _dataService: VideoDataService,
                private _navService: VideoNavigationService,
                private _changeDetectionRef: ChangeDetectorRef,
                private _activatedRoute: ActivatedRoute) {

    }

    ngOnInit(): void {
        this._activatedRoute.params.subscribe(params => {
            this.year = parseInt(params['year'], 10);

            this.categoryList$ = this._dataService
                .getCategoriesForYear(this.year);

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
