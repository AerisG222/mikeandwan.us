import { Component, Input, ChangeDetectorRef, ViewChild, AfterViewInit, OnDestroy, OnChanges } from '@angular/core';

import { ResponsiveService } from '../../ng_maw/shared/responsive.service';

import { PhotoNavigationService } from '../shared/photo-navigation.service';
import { PhotoStateService } from '../shared/photo-state.service';
import { IPhoto } from '../shared/iphoto.model';
import { ICategory } from '../shared/icategory.model';
import { PhotoListContext } from '../shared/photo-list-context.model';
import { FilterSettings } from '../shared/filter-settings.model';
import { ContainerBox } from '../shared/container-box.model';
import { IPhotoInfo } from '../shared/iphoto-info.model';
import { PhotoViewTool } from '../shared/photo-view-tool.model';
import { HelpDialogComponent } from '../help-dialog/help-dialog.component';
import { SaveDialogComponent } from '../save-dialog/save-dialog.component';

@Component({
    selector: 'app-photo-view',
    templateUrl: 'photo-view.component.html',
    styleUrls: [ 'photo-view.component.css' ]
})
export class PhotoViewComponent implements AfterViewInit, OnDestroy, OnChanges {
    private _containerBox: ContainerBox = null;
    showHistogramButton = true;
    showComments = false;
    showRatings = false;
    showExif = false;
    showHistogram = false;
    showEffects = false;
    showFullscreen = false;
    supportCssFilters = (<any>Modernizr).cssfilters;  // workaround for this missing in modernizr type definition
    rotationClassArray = ['', 'rotate_90', 'rotate_180', 'rotate_270'];
    rotationContainmentMaxWidth: string = '';
    rotationContainmentMaxHeight: string = '';
    @ViewChild(HelpDialogComponent) private _helpDialog: HelpDialogComponent;
    @ViewChild(SaveDialogComponent) private _saveDialog: SaveDialogComponent;
    @Input() context: PhotoListContext;
    @Input() showCategoryLink: boolean = false;

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

    constructor(private _changeDetectionRef: ChangeDetectorRef,
                private _stateService: PhotoStateService,
                private _navService: PhotoNavigationService,
                private _responsiveService: ResponsiveService) {
        Mousetrap.bind('right', () => this.runAndCheck(() => this.context.moveNext()));
        Mousetrap.bind('left', () => this.runAndCheck(() => this.context.movePrevious()));
        Mousetrap.bind('f', () => this.runAndCheck(() => this.toggleFullscreen()));
        Mousetrap.bind('s', () => this.runAndCheck(() => this.context.toggleSlideshow()));
        Mousetrap.bind('a', () => this.runAndCheck(() => this.rotate(-1)));
        Mousetrap.bind('d', () => this.runAndCheck(() => this.rotate(1)));
        Mousetrap.bind('x', () => this.runAndCheck(() => this.toggleExif()));
        Mousetrap.bind('c', () => this.runAndCheck(() => this.toggleComments()));
        Mousetrap.bind('r', () => this.runAndCheck(() => this.toggleRatings()));
        Mousetrap.bind('h', () => this.runAndCheck(() => this.toggleHistogram()));
        Mousetrap.bind('e', () => this.runAndCheck(() => this.toggleEffects()));
        Mousetrap.bind('?', () => this.runAndCheck(() => this.toggleHelp()));
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

        let body = <HTMLElement>document.querySelector('body');

        if (this.showFullscreen) {
            body.style.overflow = 'hidden';
            window.scrollTo(0, 0);
        } else {
            body.style.overflow = '';
        }
    }

    toggleHelp(): void {
        this._helpDialog.toggle();
    }

    rotate(direction: number): void {
        let idx = this.context.current.rotationClassIndex + direction;

        if (idx < 0) {
            this.context.current.rotationClassIndex = this.rotationClassArray.length - 1;
        } else if (idx > this.rotationClassArray.length - 1) {
            this.context.current.rotationClassIndex = 0;
        } else {
            this.context.current.rotationClassIndex = idx;
        }

        // apply the class to the canvas if we are showing histograms
        if (this.showHistogram) {
            let canvas = this.getHistogramCanvas();

            canvas.className = this.rotationClassArray[this.context.current.rotationClassIndex];
        }

        this.updateRotationContainmentDimensions();
    }

    showDownload(photoInfo: IPhotoInfo): void {
        this._saveDialog.photoInfo = photoInfo;
        this._saveDialog.show();
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
        let img = <HTMLElement>document.querySelector('div.main-image img, div.main-image canvas');

        // img will be null when in popup mode
        if (img != null) {
            let styl = filters.styleValue;

            img.style.webkitFilter = styl;
            img.style.filter = styl;
        }
    }

    private hideHistogram(): void {
        let canvas = this.getHistogramCanvas();

        if (canvas != null) {
            Pixastic.revert(canvas);
        }
    }

    private runAndCheck(func: Function) {
        func();
        this._changeDetectionRef.detectChanges();
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

        let canvas = this.getHistogramCanvas();

        if (canvas != null) {
            this.hideHistogram();
        }

        let img = this.getHistogramImage();

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
            let img = <HTMLElement>document.querySelector('div.main-image img, div.main-image canvas');

            if (img.clientWidth > this._containerBox.height) {
                this.rotationContainmentMaxWidth = this._containerBox.height + 'px';
            }

            if (img.clientHeight > this._containerBox.width) {
                this.rotationContainmentMaxHeight = this._containerBox.height + 'px';
            }
        }

        if (this.showHistogram) {
            let canvas = this.getHistogramCanvas();

            canvas.style.maxHeight = this.rotationContainmentMaxHeight;
            canvas.style.maxWidth = this.rotationContainmentMaxWidth;
        }
    }

    private updateContainerBox(): void {
        let el = document.querySelector('div.main-image > div');

        this._containerBox = new ContainerBox(el.clientWidth, el.clientHeight);
    }
}
