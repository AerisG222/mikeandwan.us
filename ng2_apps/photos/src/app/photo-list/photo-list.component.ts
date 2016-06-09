import { Component, ViewChild, AfterViewInit, OnDestroy } from '@angular/core';
import { NgIf } from '@angular/common';
import { RouteData, RouteParams } from '@angular/router-deprecated';

import { PagerComponent } from '../../../../ng_maw/src/app/pager';
import { ThumbnailListComponent, SelectedThumbnail } from '../../../../ng_maw/src/app/thumbnail-list';
import { ResponsiveService } from '../../../../ng_maw/src/app/shared';

import { Config, ModeRouteInfo, PhotoStateService, PhotoDataService, PhotoThumbnailInfo, PhotoSource, PhotoSourceFactory, PhotoListContext, RouteMode, RandomPhotoListContext, Photo } from '../shared';
import { PhotoViewComponent } from '../photo-view';
import { MapViewComponent } from '../map-view';
import { PhotoDialogComponent } from '../photo-dialog';

@Component({
    moduleId: module.id,
    selector: 'app-photo-list',
    directives: [NgIf, PagerComponent, ThumbnailListComponent, PhotoViewComponent, MapViewComponent, PhotoDialogComponent],
    templateUrl: 'photo-list.component.html',
    styleUrls: ['photo-list.component.css']
})
export class PhotoListComponent implements AfterViewInit, OnDestroy {
    @ViewChild(PagerComponent) pager: PagerComponent;
    @ViewChild(ThumbnailListComponent) thumbnailList: ThumbnailListComponent;
    @ViewChild(PhotoDialogComponent) private _photoDialog: PhotoDialogComponent;
    private _modeInfo: ModeRouteInfo = null;
    private _photoSource: PhotoSource = null;
    showMapView: boolean = false;
    showPhotoView: boolean = false;
    context: PhotoListContext;

    constructor(private _dataService: PhotoDataService,
        private _stateService: PhotoStateService,
        private _responsiveService: ResponsiveService,
        photoSourceFactory: PhotoSourceFactory,
        routeParams: RouteParams,
        routeData: RouteData) {
        this._modeInfo = <ModeRouteInfo>routeData.data;
        this._photoSource = photoSourceFactory.create(routeData, routeParams);
        this.showPhotoView = _stateService.config.displayMode === Config.DISPLAY_MODE_INLINE;
    }

    ngAfterViewInit() {
        this._responsiveService.onBreakpointChange.subscribe((breakpoint: string) => {
            if (this._responsiveService._currBp === ResponsiveService.BP_XS) {
                this.showMapView = false;
                this.showPhotoView = false;
            }
            else {
                if (!this.showMapView) {
                    // if we are resizing from xs, restore the photo view based on their preferences
                    this.showPhotoView = this._stateService.config.displayMode === Config.DISPLAY_MODE_INLINE;
                }
            }
        });

        this.thumbnailList.setRowCountPerPage(this._stateService.config.rowsPerPage);

        this.thumbnailList.itemsPerPageUpdated.subscribe((x: any) => {
            this.updatePager();
        });

        this._stateService.showPreferencesEventEmitter.subscribe((x: any) => {
            this.context.terminateSlideshow();
        });

        this._stateService.configUpdatedEventEmitter.subscribe((config: Config) => {
            this.onConfigChange(config);
        });

        this._stateService.toggleMapsEventEmitter.subscribe((showMaps: boolean) => {
            this.onToggleMapsView(showMaps);
        });

        // TODO: look at refactoring photosource / context to simplify + provide better separation
        this._photoSource
            .getPhotos()
            .subscribe(photos => {
                if (this._modeInfo.mode === RouteMode.Random) {
                    this.context = new RandomPhotoListContext(photos, this._modeInfo.mode, this._stateService, this._photoSource);

                    (<RandomPhotoListContext>this.context).photoAddedEventEmitter.subscribe((photo: Photo) => {
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

                this.context.photoUpdated.subscribe((idx: number) => this.onPhotoUpdated(idx));

                let thumbs = photos.map(x => new PhotoThumbnailInfo(x.photo.thumbnailInfo.path,
                    x.photo.thumbnailInfo.height,
                    x.photo.thumbnailInfo.width,
                    x));

                this.thumbnailList.setItemList(thumbs);
                this.updatePager();
            });
    }

    ngOnDestroy(): void {
        this.showPhotoView = false;
        this.context.terminateSlideshow();
    }

    onChangePage(pageIndex: number): void {
        this.thumbnailList.setPageDisplayedIndex(pageIndex);
    }

    onThumbnailSelected(item: SelectedThumbnail): void {
        this.context.moveTo(item.index);
    }

    onPhotoUpdated(index: number): void {
        this.pager.activatePage(this.pager.calcActivePage(index, this.thumbnailList.itemsPerPage));
        this.thumbnailList.setItemSelectedIndex(index);

        if (!this.showPhotoView && !this.showMapView) {
            this._photoDialog.show();
        }
    }

    onConfigChange(config: Config): void {
        this.showPhotoView = config.displayMode === Config.DISPLAY_MODE_INLINE;
        this.thumbnailList.setRowCountPerPage(config.rowsPerPage);
    }

    onToggleMapsView(showMap: boolean): void {
        this.showMapView = showMap;
        this.showPhotoView = !showMap;
    }

    private updatePager() {
        this.pager.setPageCount(this.pager.calcPageCount(this.thumbnailList.itemList.length, this.thumbnailList.itemsPerPage));
        this.pager.setActivePage(this.thumbnailList.pageDisplayedIndex);
    }
}
