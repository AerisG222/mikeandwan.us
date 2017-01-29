import { Component, ViewChild, AfterViewInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { PagerComponent } from '../../ng_maw/pager/pager.component';
import { ThumbnailListComponent } from '../../ng_maw/thumbnail-list/thumbnail-list.component';
import { SelectedThumbnail } from '../../ng_maw/thumbnail-list/selected-thumbnail.model';
import { ResponsiveService } from '../../ng_maw/shared';

import { Config } from '../shared/config.model';
import { ModeRouteInfo } from '../shared/mode-route-info.model';
import { PhotoStateService } from '../shared/photo-state.service';
import { PhotoDataService } from '../shared/photo-data.service';
import { PhotoThumbnailInfo } from '../shared/photo-thumbnail-info.model';
import { PhotoSource } from '../shared/photo-source.model';
import { PhotoSourceFactory } from '../shared/photo-source-factory.model';
import { PhotoListContext } from '../shared/photo-list-context.model';
import { RouteMode } from '../shared/route-mode.model';
import { RandomPhotoListContext } from '../shared/random-photo-list-context.model';
import { Photo } from '../shared/photo.model';
import { PhotoDialogComponent } from '../photo-dialog/photo-dialog.component';

@Component({
    selector: 'app-photo-list',
    templateUrl: './photo-list.component.html',
    styleUrls: [ './photo-list.component.css' ]
})
export class PhotoListComponent implements AfterViewInit, OnDestroy {
    @ViewChild(PagerComponent) pager: PagerComponent;
    @ViewChild(ThumbnailListComponent) thumbnailList: ThumbnailListComponent;
    @ViewChild(PhotoDialogComponent) private _photoDialog: PhotoDialogComponent;
    private _modeInfo: RouteMode = null;
    private _photoSource: PhotoSource = null;
    showMapView = false;
    showPhotoView = false;
    context: PhotoListContext;

    constructor(private _dataService: PhotoDataService,
                private _stateService: PhotoStateService,
                private _responsiveService: ResponsiveService,
                private _activatedRoute: ActivatedRoute,
                photoSourceFactory: PhotoSourceFactory) {
        this._activatedRoute.params.subscribe(params => {
            this._activatedRoute.data.subscribe(data => {
                this._modeInfo = data[ModeRouteInfo.PARAM_MODE];
                this._photoSource = photoSourceFactory.create(data, params);
                this.showPhotoView = _stateService.config.displayMode === Config.DISPLAY_MODE_INLINE;
            });
        });
    }

    ngAfterViewInit() {
        this._responsiveService.onBreakpointChange.subscribe((breakpoint: string) => {
            if (this._responsiveService._currBp === ResponsiveService.BP_XS) {
                this.showMapView = false;
                this.showPhotoView = false;
            } else {
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
                if (this._modeInfo === RouteMode.Random) {
                    this.context = new RandomPhotoListContext(photos, this._modeInfo, this._stateService, this._photoSource);

                    (<RandomPhotoListContext>this.context).photoAddedEventEmitter.subscribe((photo: Photo) => {
                        const thumb = new PhotoThumbnailInfo(photo.photo.xsInfo.path,
                            photo.photo.xsInfo.height,
                            photo.photo.xsInfo.width,
                            photo);
                        this.thumbnailList.addItem(thumb);
                        this.updatePager();
                    });
                } else {
                    this.context = new PhotoListContext(photos, this._modeInfo, this._stateService);
                }

                this.context.photoUpdated.subscribe((idx: number) => this.onPhotoUpdated(idx));

                const thumbs = photos.map(x => new PhotoThumbnailInfo(x.photo.xsInfo.path,
                    x.photo.xsInfo.height,
                    x.photo.xsInfo.width,
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
