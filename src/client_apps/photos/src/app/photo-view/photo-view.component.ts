import { Component, Input, AfterViewInit, OnDestroy, OnChanges, NgZone } from '@angular/core';
import { trigger, state, style, animate, transition } from '@angular/animations';

import * as Mousetrap from 'mousetrap';

import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';

import { ResponsiveService } from '../services/responsive.service';
import { SvgIcon } from 'maw-common';
import { PhotoNavigationService } from '../services/photo-navigation.service';
import { PhotoStateService } from '../services/photo-state.service';
import { IPhoto } from '../models/iphoto.model';
import { ICategory } from '../models/icategory.model';
import { PhotoListContext } from '../models/photo-list-context.model';
import { FilterSettings } from '../models/filter-settings.model';
import { ContainerBox } from '../models/container-box.model';
import { IPhotoInfo } from '../models/iphoto-info.model';
import { HelpDialogComponent } from '../help-dialog/help-dialog.component';
import { SaveDialogComponent } from '../save-dialog/save-dialog.component';
import { PhotoViewTool } from '../models/photo-view-tool.model';

@Component({
    selector: 'app-photo-view',
    templateUrl: './photo-view.component.html',
    styleUrls: [ './photo-view.component.css' ],
    animations: [
        trigger('fadeInOut', [
            state('in', style({opacity: 1})),
            transition('void => *', [
                style({opacity: 0}),
                animate(320)
            ])
        ])
    ]
})
export class PhotoViewComponent implements AfterViewInit, OnDestroy, OnChanges {
    private _containerBox: ContainerBox = null;
    private _helpModal: NgbModalRef = null;
    svgIcon = SvgIcon;
    showHistogramButton = true;
    showComments = false;
    showRatings = false;
    showExif = false;
    showHistogram = false;
    showEffects = false;
    showFullscreen = false;
    supportCssFilters = (<any>Modernizr).cssfilters;  // workaround for this missing in modernizr type definition
    rotationClassArray = ['', 'rotate_90', 'rotate_180', 'rotate_270'];
    rotationContainmentMaxWidth = '';
    rotationContainmentMaxHeight = '';
    @Input() context: PhotoListContext;
    @Input() showCategoryLink = false;

    get photo(): IPhoto {
        try {
            return this.context.current.photo;
        } catch (ex) {
            return null;
        }
    }

    get category(): ICategory {
        try {
            return this.context.current.category;
        } catch (ex) {
            return null;
        }
    }

    constructor(private _zone: NgZone,
                private _modal: NgbModal,
                private _stateService: PhotoStateService,
                private _navService: PhotoNavigationService,
                private _responsiveService: ResponsiveService) {
        Mousetrap.bind('right', () => this._zone.run(() => this.context.moveNext()));
        Mousetrap.bind('left', () => this._zone.run(() => this.context.movePrevious()));
        Mousetrap.bind('f', () => this._zone.run(() => this.toggleFullscreen()));
        Mousetrap.bind('s', () => this._zone.run(() => this.context.toggleSlideshow()));
        Mousetrap.bind('a', () => this._zone.run(() => this.rotate(-1)));
        Mousetrap.bind('d', () => this._zone.run(() => this.rotate(1)));
        Mousetrap.bind('x', () => this._zone.run(() => this.toggleExif()));
        Mousetrap.bind('c', () => this._zone.run(() => this.toggleComments()));
        Mousetrap.bind('r', () => this._zone.run(() => this.toggleRatings()));
        Mousetrap.bind('h', () => this._zone.run(() => this.toggleHistogram()));
        Mousetrap.bind('e', () => this._zone.run(() => this.toggleEffects()));
        Mousetrap.bind('?', () => this._zone.run(() => this.toggleHelp()));
    }

    ngOnChanges(): void {
        this.context.photoUpdated.subscribe((idx: number) => this.onPhotoUpdated(idx));
    }

    ngAfterViewInit(): void {
        this.updateContainerBox();
        this.updateRotationContainmentDimensions();

        // if a user activates filter effects, then goes to map views, then returns here on a
        // different image, we need to clear filters so they are correct for the new image in this view
        if (this.context.currentIndex >= 0) {
            this.onPhotoUpdated(this.context.currentIndex);
        }
    }

    ngOnDestroy(): void {
        Mousetrap.reset();

        if (this.showHistogram) {
            this.toggleHistogram();
        }

        this.onFiltersUpdated(new FilterSettings());
    }

    toggleComments(): void {
        this.hideAllExcept(PhotoViewTool.Comments);

        this.showComments = !this.showComments;
    }

    toggleRatings(): void {
        this.hideAllExcept(PhotoViewTool.Ratings);

        this.showRatings = !this.showRatings;
    }

    toggleExif(): void {
        this.hideAllExcept(PhotoViewTool.Exif);

        this.showExif = !this.showExif;
    }

    toggleEffects(): void {
        this.hideAllExcept(PhotoViewTool.Effects);

        this.showEffects = !this.showEffects;
    }

    toggleHistogram(): void {
        this.showHistogram = !this.showHistogram;

        if (this.showHistogram) {
            this.renderHistogram();
        } else {
            this.hideHistogram();
        }
    }

    toggleFullscreen(): void {
        this.showFullscreen = !this.showFullscreen;

        const body = <HTMLElement>document.querySelector('body');

        if (this.showFullscreen) {
            body.style.overflow = 'hidden';
            window.scrollTo(0, 0);
        } else {
            body.style.overflow = '';
        }
    }

    toggleHelp(): void {
        if (this._helpModal === null) {
            this._helpModal = this._modal.open(HelpDialogComponent);
        } else {
            this._helpModal.dismiss();
            this._helpModal = null;
        }
    }

    rotate(direction: number): void {
        const idx = this.context.current.rotationClassIndex + direction;

        if (idx < 0) {
            this.context.current.rotationClassIndex = this.rotationClassArray.length - 1;
        } else if (idx > this.rotationClassArray.length - 1) {
            this.context.current.rotationClassIndex = 0;
        } else {
            this.context.current.rotationClassIndex = idx;
        }

        // apply the class to the canvas if we are showing histograms
        if (this.showHistogram) {
            const canvas = this.getHistogramCanvas();

            canvas.className = this.rotationClassArray[this.context.current.rotationClassIndex];
        }

        this.updateRotationContainmentDimensions();
    }

    showDownload(photoInfo: IPhotoInfo): void {
        const modal = this._modal.open(SaveDialogComponent);
        modal.componentInstance.photoInfo = photoInfo;
    }

    getPhotoPath(): string {
        if (this.context == null || this.context.current == null) {
            return '';
        }

        if (this._responsiveService.getBreakpoint() === ResponsiveService.BP_LG) {
            return this.photo.mdInfo.path;
        } else {
            return this.photo.smInfo.path;
        }
    }

    onPhotoUpdated(index: number): void {
        this.onFiltersUpdated(this.context.current.filters);
        this.renderHistogram();
        this.updateRotationContainmentDimensions();
    }

    onFiltersUpdated(filters: FilterSettings): void {
        const img = <HTMLElement>document.querySelector('div.main-image img, div.main-image canvas');

        // img will be null when in popup mode
        if (img != null) {
            const styl = filters.styleValue;

            img.style.webkitFilter = styl;
            img.style.filter = styl;
        }
    }

    private hideHistogram(): void {
        const canvas = this.getHistogramCanvas();

        if (canvas != null) {
            Pixastic.revert(canvas);
        }
    }

    private getHistogramCanvas(): HTMLCanvasElement {
        return <HTMLCanvasElement>document.querySelector('div.main-image canvas');
    }

    private getHistogramImage(): HTMLImageElement {
        return <HTMLImageElement>document.querySelector('div.main-image img');
    }

    private renderHistogram(): void {
        if (!this.showHistogram) {
            return;
        }

        const canvas = this.getHistogramCanvas();

        if (canvas != null) {
            this.hideHistogram();
        }

        const img = this.getHistogramImage();

        Pixastic.process(img, 'histogram', { average: true, paint: true, color: 'rgba(255,255,255,0.5)' });
    }

    private hideAllExcept(itemToSkip: PhotoViewTool): void {
        if (itemToSkip !== PhotoViewTool.Comments && this.showComments) {
            this.toggleComments();
        }
        if (itemToSkip !== PhotoViewTool.Ratings && this.showRatings) {
            this.toggleRatings();
        }
        if (itemToSkip !== PhotoViewTool.Exif && this.showExif) {
            this.toggleExif();
        }
        if (itemToSkip !== PhotoViewTool.Effects && this.showEffects) {
            this.toggleEffects();
        }
    }

    private updateRotationContainmentDimensions(): void {
        this.rotationContainmentMaxHeight = '';
        this.rotationContainmentMaxWidth = '';

        // images are already scaled to fit w/o rotation, so no need to do anything in the standard orientation
        if (this.context.current.rotationClassIndex % 2 === 1) {
            const img = <HTMLElement>document.querySelector('div.main-image img, div.main-image canvas');

            if (img.clientWidth > this._containerBox.height) {
                this.rotationContainmentMaxWidth = this._containerBox.height + 'px';
            }

            if (img.clientHeight > this._containerBox.width) {
                this.rotationContainmentMaxHeight = this._containerBox.height + 'px';
            }
        }

        if (this.showHistogram) {
            const canvas = this.getHistogramCanvas();

            canvas.style.maxHeight = this.rotationContainmentMaxHeight;
            canvas.style.maxWidth = this.rotationContainmentMaxWidth;
        }
    }

    private updateContainerBox(): void {
        const el = document.querySelector('div.main-image > div');

        this._containerBox = new ContainerBox(el.clientWidth, el.clientHeight);
    }
}
