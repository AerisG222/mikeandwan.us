import { Component, ViewChild, ChangeDetectorRef, AfterViewInit } from '@angular/core';
import { RouteParams } from '@angular/router';
import { SizeService, VideoDataService, VideoStateService } from '../shared/index';
import { ICategory, CategoryThumbnailInfo, VideoNavigationService } from '../shared/';
import { PagerComponent } from '../../../../ng_maw/src/app/pager/index';
import { ThumbnailListComponent, SelectedThumbnail } from '../../../../ng_maw/src/app/thumbnail-list/index';

@Component({
    moduleId: module.id,
    selector: 'app-category-list',
    templateUrl: 'category-list.component.html',
    styleUrls: ['category-list.component.css']
})
export class CategoryListComponent implements AfterViewInit {
    @ViewChild(PagerComponent) pager : PagerComponent;
    @ViewChild(ThumbnailListComponent) thumbnailList : ThumbnailListComponent;
    year : number = -1;
    categories : Array<ICategory> = [];

    constructor(private _sizeService : SizeService, 
                private _dataService : VideoDataService,
                private _stateService : VideoStateService,
                private _navService : VideoNavigationService,
                private _changeDetectionRef : ChangeDetectorRef,
                params : RouteParams) {
        this.year = parseInt(params.get('year'), 10);
    }

    ngAfterViewInit() : void {
        this.thumbnailList.setRowCountPerPage(2);
        
        this.thumbnailList.itemsPerPageUpdated.subscribe((x : any) => {
            this.updatePager();
        });
        
        this._dataService.getCategoriesForYear(this.year)
            .subscribe(
                (data : Array<ICategory>) => this.setCategories(data),
                (err : any) => console.error(`there was an error: ${err}`)
            );
        
        this._changeDetectionRef.detectChanges();
    }
    
    setCategories(categories : Array<ICategory>) : void {
        this.categories = categories;
        
        let thumbnails : Array<CategoryThumbnailInfo> = categories.map((cat : ICategory) => 
            new CategoryThumbnailInfo(cat.teaserThumbnail.path,
                this._sizeService.getThumbHeight(cat.teaserThumbnail.width, cat.teaserThumbnail.height),
                this._sizeService.getThumbWidth(cat.teaserThumbnail.width, cat.teaserThumbnail.height),
                cat,
                cat.name,
                null
            )
        );
        
        this.thumbnailList.setItemList(thumbnails);
        this.pager.setPageCount(Math.ceil(thumbnails.length / this.thumbnailList.itemsPerPage));
    }
    
    onChangePage(pageIndex : number) : void {
        this.thumbnailList.setPageDisplayedIndex(pageIndex);
    }

    onThumbnailSelected(item : SelectedThumbnail) : void {
        if(item.index >= 0 && this.categories.length > item.index) {
            let cat : ICategory = (<CategoryThumbnailInfo>item.thumbnail).category;
            this._navService.gotoVideoList(cat.year, cat);
        }
    }
    
    private updatePager() {
        this.pager.setPageCount(this.pager.calcPageCount(this.thumbnailList.itemList.length, this.thumbnailList.itemsPerPage));
        this.pager.setActivePage(this.thumbnailList.pageDisplayedIndex);
    }
}
