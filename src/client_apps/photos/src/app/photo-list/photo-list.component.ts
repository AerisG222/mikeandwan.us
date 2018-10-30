import { Component, AfterViewInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { trigger, state, style, transition, useAnimation } from '@angular/animations';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

import { fadeIn, fadeOut } from 'maw-common';
import { Config } from '../models/config.model';
import { ModeRouteInfo } from '../models/mode-route-info.model';
import { PhotoStateService } from '../services/photo-state.service';
import { PhotoSource } from '../models/photo-source.model';
import { PhotoSourceFactory } from '../models/photo-source-factory.model';
import { PhotoListContext } from '../models/photo-list-context.model';
import { RouteMode } from '../models/route-mode.model';
import { RandomPhotoListContext } from '../models/random-photo-list-context.model';
import { PhotoDialogComponent } from '../photo-dialog/photo-dialog.component';
import { ResponsiveService } from '../services/responsive.service';
import { Photo } from '../models/photo.model';

@Component({
    selector: 'app-photo-list',
    templateUrl: './photo-list.component.html',
    styleUrls: [ './photo-list.component.css' ],
    animations: [
        trigger('fadeInOut', [
            state('in', style({opacity: 1})),
            transition(':enter', useAnimation(fadeIn)),
            transition(':leave', useAnimation(fadeOut))
        ])
    ]
})
export class PhotoListComponent implements AfterViewInit, OnDestroy {
    private _modeInfo: RouteMode = null;
    private _photoSource: PhotoSource = null;
    showMapView = false;
    showPhotoView = false;
    context: PhotoListContext;
    page = 1;
    cardsPerPage: number;

    constructor(private _changeDetectorRef: ChangeDetectorRef,
                private _modalService: NgbModal,
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
        this.setCardsPerPage(this._stateService.config);

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

        this._stateService.showPreferencesEventEmitter.subscribe((x: any) => {
            this.context.terminateSlideshow();
        });

        this._stateService.configUpdatedEventEmitter.subscribe((config: Config) => {
            this.setCardsPerPage(config);
            this.setPhotoView(config);
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

                    /*
                    (<RandomPhotoListContext>this.context).photoAddedEventEmitter.subscribe((photo: Photo) => {
                        const thumb = new PhotoThumbnailInfo(photo.photo.xsInfo.path,
                            photo.photo.xsInfo.height,
                            photo.photo.xsInfo.width,
                            photo);
                        this.thumbnailList.addItem(thumb);
                    });
                    */
                } else {
                    this.context = new PhotoListContext(photos, this._modeInfo, this._stateService);
                }

                this.context.photoUpdated.subscribe((idx: number) => this.onPhotoUpdated(idx));
            });

        this._changeDetectorRef.detectChanges();
    }

    ngOnDestroy(): void {
        this.showPhotoView = false;
        this.context.terminateSlideshow();
    }

    onPhotoSelected(photo: Photo): void {
        this.context.moveToPhoto(photo);
    }

    onPhotoUpdated(index: number): void {
        this.updatePageIfNeeded(index);

        if (!this.showPhotoView && !this.showMapView) {
            const modal = this._modalService.open(PhotoDialogComponent);
            modal.componentInstance.photo = this.context.current.photo;
        }
    }

    onToggleMapsView(showMap: boolean): void {
        this.showMapView = showMap;
        this.showPhotoView = !showMap;
    }

    updatePageIfNeeded(index: number): void {
        this.page = Math.floor(index / this.cardsPerPage) + 1;
    }

    setCardsPerPage(config: Config): void {
        this.page = 1;
        this.cardsPerPage = config.thumbnailsPerPage;
    }

    setPhotoView(config: Config): void {
        this.showPhotoView = config.displayMode === Config.DISPLAY_MODE_INLINE;
    }
}
