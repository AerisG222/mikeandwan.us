import { Component, ViewChild, ChangeDetectorRef, AfterViewInit } from '@angular/core';
import { RouteParams } from '@angular/router-deprecated';
import { SizeService } from '../services/SizeService';
import { VideoDataService } from '../services/VideoDataService';
import { VideoStateService } from '../services/VideoStateService';
import { ICategory } from '../interfaces/ICategory';
import { Pager } from '../../ng_maw/pager/Pager';
import { ThumbnailList } from '../../ng_maw/thumbnailList/ThumbnailList';
import { CategoryThumbnailInfo } from '../models/CategoryThumbnailInfo';
import { SelectedThumbnail } from '../../ng_maw/thumbnailList/SelectedThumbnail';
import { VideoNavigationService } from '../services/VideoNavigationService';

@Component({
    selector: 'categorylist',	
    directives: [ Pager, ThumbnailList ],
    templateUrl: '/js/videos/components/CategoryList.html'
})
export class CategoryList implements AfterViewInit {
    @ViewChild(Pager) pager : Pager;
    @ViewChild(ThumbnailList) thumbnailList : ThumbnailList;
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
