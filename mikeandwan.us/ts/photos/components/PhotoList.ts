import { Component, ViewChild, AfterViewInit, OnDestroy } from '@angular/core';
import { NgIf } from '@angular/common';
import { RouteData, RouteParams } from '@angular/router-deprecated';
import { Pager } from '../../ng_maw/pager/Pager';
import { ThumbnailList } from '../../ng_maw/thumbnailList/ThumbnailList';
import { SelectedThumbnail } from '../../ng_maw/thumbnailList/SelectedThumbnail';
import { PhotoThumbnailInfo } from '../models/PhotoThumbnailInfo';
import { PhotoDataService } from '../services/PhotoDataService';
import { PhotoStateService } from '../services/PhotoStateService';
import { ResponsiveService } from '../../ng_maw/services/ResponsiveService';
import { ModeRouteInfo } from '../models/ModeRouteInfo';
import { PhotoSource } from '../models/PhotoSource';
import { PhotoSourceFactory } from '../models/PhotoSourceFactory';
import { PhotoView } from './PhotoView';
import { MapView } from './MapView';
import { Config } from '../models/Config';
import { PhotoListContext } from '../models/PhotoListContext';
import { PhotoDialog } from './PhotoDialog';
import { RouteMode } from '../models/RouteMode';
import { RandomPhotoListContext } from '../models/RandomPhotoListContext';
import { Photo } from '../models/Photo';

@Component({
    selector: 'photolist',	
    directives: [ NgIf, Pager, ThumbnailList, PhotoView, MapView, PhotoDialog ],
    templateUrl: '/js/photos/components/PhotoList.html'
})
export class PhotoList implements AfterViewInit, OnDestroy {
    @ViewChild(Pager) pager : Pager;
    @ViewChild(ThumbnailList) thumbnailList : ThumbnailList;
    @ViewChild(PhotoDialog) private _photoDialog : PhotoDialog;
    private _modeInfo : ModeRouteInfo = null;
    private _photoSource : PhotoSource = null;
    showMapView : boolean = false;
    showPhotoView : boolean = false;
    context : PhotoListContext;
    
    constructor(private _dataService : PhotoDataService, 
                private _stateService : PhotoStateService,
                private _responsiveService : ResponsiveService,
                photoSourceFactory : PhotoSourceFactory,
                routeParams : RouteParams,
                routeData : RouteData) {
        this._modeInfo = <ModeRouteInfo>routeData.data;
        this._photoSource = photoSourceFactory.create(routeData, routeParams);
        this.showPhotoView = _stateService.config.displayMode === Config.DISPLAY_MODE_INLINE;
    }
    
    ngAfterViewInit() {
        this._responsiveService.onBreakpointChange.subscribe((breakpoint : string) => {
            if(this._responsiveService._currBp === ResponsiveService.BP_XS) {
                this.showMapView = false;
                this.showPhotoView = false;
            }
            else {
                if(!this.showMapView) {
                    // if we are resizing from xs, restore the photo view based on their preferences
                    this.showPhotoView = this._stateService.config.displayMode === Config.DISPLAY_MODE_INLINE;
                }
            }
        });
        
        this.thumbnailList.setRowCountPerPage(this._stateService.config.rowsPerPage);
        
        this.thumbnailList.itemsPerPageUpdated.subscribe((x : any) => {
            this.updatePager();
        });
        
        this._stateService.showPreferencesEventEmitter.subscribe((x : any) => {
            this.context.terminateSlideshow();
        });
        
        this._stateService.configUpdatedEventEmitter.subscribe((config : Config) => {
            this.onConfigChange(config);
        });
        
        this._stateService.toggleMapsEventEmitter.subscribe((showMaps : boolean) => {
            this.onToggleMapsView(showMaps);
        });
        
        // TODO: look at refactoring photosource / context to simplify + provide better separation
        this._photoSource
            .getPhotos()
            .subscribe(photos => {
                if(this._modeInfo.mode === RouteMode.Random) {
                    this.context = new RandomPhotoListContext(photos, this._modeInfo.mode, this._stateService, this._photoSource);

                    (<RandomPhotoListContext>this.context).photoAddedEventEmitter.subscribe((photo : Photo) => {
                        let thumb = new PhotoThumbnailInfo(photo.photo.thumbnailInfo.path, 
                                                           photo.photo.thumbnailInfo.height, 
                                                           photo.photo.thumbnailInfo.width, 
                                                           photo);
                        this.thumbnailList.addItem(thumb);
                        this.updatePager();
                    });
                }
                else {
                    this.context = new PhotoListContext(photos, this._modeInfo.mode, this._stateService);
                }
                
                this.context.photoUpdated.subscribe((idx : number) => this.onPhotoUpdated(idx));
                
                let thumbs = photos.map(x => new PhotoThumbnailInfo(x.photo.thumbnailInfo.path, 
                                                                    x.photo.thumbnailInfo.height, 
                                                                    x.photo.thumbnailInfo.width, 
                                                                    x));
                
                this.thumbnailList.setItemList(thumbs);
                this.updatePager();
            });
    }
    
    ngOnDestroy() : void {
        this.showPhotoView = false;
        this.context.terminateSlideshow();
    }
    
    onChangePage(pageIndex : number) : void {
        this.thumbnailList.setPageDisplayedIndex(pageIndex);
    }
    
    onThumbnailSelected(item : SelectedThumbnail) : void {
        this.context.moveTo(item.index);
    }
    
    onPhotoUpdated(index : number) : void {
        this.pager.activatePage(this.pager.calcActivePage(index, this.thumbnailList.itemsPerPage));
        this.thumbnailList.setItemSelectedIndex(index);
        
        if(!this.showPhotoView && !this.showMapView) {
            this._photoDialog.show();
        }
    }
    
    onConfigChange(config : Config) : void {
        this.showPhotoView = config.displayMode === Config.DISPLAY_MODE_INLINE;
        this.thumbnailList.setRowCountPerPage(config.rowsPerPage);
    }
    
    onToggleMapsView(showMap : boolean) : void {
        this.showMapView = showMap;
        this.showPhotoView = !showMap;
    }
    
    private updatePager() {
        this.pager.setPageCount(this.pager.calcPageCount(this.thumbnailList.itemList.length, this.thumbnailList.itemsPerPage));
        this.pager.setActivePage(this.thumbnailList.pageDisplayedIndex);
    }
}
