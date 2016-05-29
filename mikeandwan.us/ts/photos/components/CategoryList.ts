import { Component, ViewChild, AfterViewInit } from '@angular/core';
import { RouteParams } from '@angular/router-deprecated';
import { ModeRouteInfo } from '../models/ModeRouteInfo';
import { Pager } from '../../ng_maw/pager/Pager';
import { ThumbnailList } from '../../ng_maw/thumbnailList/ThumbnailList';
import { PhotoNavigationService } from '../services/PhotoNavigationService';
import { PhotoStateService } from '../services/PhotoStateService';
import { CategoryThumbnailInfo } from '../models/CategoryThumbnailInfo';
import { SelectedThumbnail } from '../../ng_maw/thumbnailList/SelectedThumbnail';
import { Config } from '../models/Config';

@Component({
    selector: 'categorylist',	
    directives: [ Pager, ThumbnailList ],
    templateUrl: '/js/photos/components/CategoryList.html'
})
export class CategoryList implements AfterViewInit {
    @ViewChild(Pager) pager : Pager;
    @ViewChild(ThumbnailList) thumbnailList : ThumbnailList;
    private _year : number = null;
    
    constructor(routeParams : RouteParams,
                private _stateService : PhotoStateService, 
                private _navService : PhotoNavigationService) {
        this._year = parseInt(routeParams.get(ModeRouteInfo.PARAM_YEAR), 10);
    }
    
    ngAfterViewInit() {
        this.thumbnailList.setRowCountPerPage(this._stateService.config.rowsPerPage);
        
        this.thumbnailList.itemsPerPageUpdated.subscribe((x : any) => {
            this.updatePager();
        });
        
        this._stateService.configUpdatedEventEmitter.subscribe((config : Config) => {
            this.onConfigChange(config);
        });
        
        this._navService.getCategoryDestinations(this._year)
            .subscribe(destinations => {
                let d = destinations.map(x => new CategoryThumbnailInfo(x.category.teaserPhotoInfo.path, 
                    x.category.teaserPhotoInfo.height,
                    x.category.teaserPhotoInfo.width,
                    x, 
                    x.category.name,
                    x.category.hasGpsData ? 'fa-map-marker' : null));
                    
                this.thumbnailList.setItemList(d);
                this.pager.setPageCount(this.pager.calcPageCount(d.length, this.thumbnailList.itemsPerPage));
    
                let lastIndex = this._stateService.lastCategoryIndex;
        
                if(lastIndex > 0) {
                    let page = Math.floor(lastIndex / this.thumbnailList.itemsPerPage);
        
                    this.thumbnailList.setPageDisplayedIndex(page);
                    this.pager.setActivePage(page);
                }
            });
    }
    
    onChangePage(pageIndex : number) {
        if(pageIndex >= 0) {
            this.thumbnailList.setPageDisplayedIndex(pageIndex);
        }
    }
    
    onThumbnailSelected(thumbInfo : SelectedThumbnail) {
        if(thumbInfo !== null) {
            this._stateService.lastCategoryIndex = thumbInfo.index;
            this._navService.gotoCategoryPhotoList((<CategoryThumbnailInfo>thumbInfo.thumbnail).category);
        }
    }
    
    onConfigChange(config : Config) : void {
        this.thumbnailList.setRowCountPerPage(config.rowsPerPage);
        this.updatePager();
    }
    
    private updatePager() {
        this.pager.setPageCount(this.pager.calcPageCount(this.thumbnailList.itemList.length, this.thumbnailList.itemsPerPage));
        this.pager.setActivePage(this.thumbnailList.pageDisplayedIndex);
    }
}
