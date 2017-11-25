import { Component, ViewChild, AfterViewInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { animate, state, style, transition, trigger, useAnimation } from '@angular/animations';

import { NgbPagination } from '@ng-bootstrap/ng-bootstrap';

import { fadeIn, fadeOut } from '../../ng_maw/shared/animation';
import { ThumbnailListComponent } from '../../ng_maw/thumbnail-list/thumbnail-list.component';
import { SelectedThumbnail } from '../../ng_maw/thumbnail-list/selected-thumbnail.model';
import { SvgIcon } from '../../ng_maw/svg-icon/svg-icon.enum';

import { Config, ModeRouteInfo, PhotoNavigationService, PhotoStateService, CategoryThumbnailInfo } from '../shared';

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
    @ViewChild(NgbPagination) pager: NgbPagination;
    @ViewChild(ThumbnailListComponent) thumbnailList: ThumbnailListComponent;
    private _year: number = null;

    constructor(private _stateService: PhotoStateService,
                private _navService: PhotoNavigationService,
                private _activatedRoute: ActivatedRoute) {

    }

    ngAfterViewInit() {
        this._activatedRoute.params.subscribe(params => {
            this._year = parseInt(params[ModeRouteInfo.PARAM_YEAR], 10);

            this.thumbnailList.setRowCountPerPage(this._stateService.config.rowsPerPage);

            this.thumbnailList.itemsPerPageUpdated.subscribe((x: any) => {
                this.updatePager();
            });

            this._stateService.configUpdatedEventEmitter.subscribe((config: Config) => {
                this.onConfigChange(config);
            });

            this._navService.getCategoryDestinations(this._year)
                .subscribe(destinations => {
                    const d = destinations.map(x => new CategoryThumbnailInfo(x.category.teaserPhotoInfo.path,
                        x.category.teaserPhotoInfo.height,
                        x.category.teaserPhotoInfo.width,
                        x,
                        x.category.name,
                        x.category.hasGpsData ? SvgIcon.MapMarker : null));

                    this.thumbnailList.setItemList(d);
                    // this.pager.setPageCount(this.pager.calcPageCount(d.length, this.thumbnailList.itemsPerPage));

                    const lastIndex = this._stateService.lastCategoryIndex;

                    if (lastIndex > 0) {
                        const page = Math.floor(lastIndex / this.thumbnailList.itemsPerPage);

                        this.thumbnailList.setPageDisplayedIndex(page);
                        this.pager.page = page;
                    }
                });
        });
    }

    onChangePage(page: number) {
        if (page >= 0) {
            this.thumbnailList.setPageDisplayedIndex(page - 1);
        }
    }

    onThumbnailSelected(thumbInfo: SelectedThumbnail) {
        if (thumbInfo !== null) {
            this._stateService.lastCategoryIndex = thumbInfo.index;
            this._navService.gotoCategoryPhotoList((<CategoryThumbnailInfo>thumbInfo.thumbnail).category);
        }
    }

    onConfigChange(config: Config): void {
        this.thumbnailList.setRowCountPerPage(config.rowsPerPage);
        this.updatePager();
    }

    private updatePager() {
        this.pager.page = this.thumbnailList.pageDisplayedIndex + 1;
    }
}
