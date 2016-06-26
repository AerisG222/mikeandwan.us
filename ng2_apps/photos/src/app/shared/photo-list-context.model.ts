import { EventEmitter } from '@angular/core';

import { Photo } from './photo.model';
import { PhotoStateService } from './photo-state.service';
import { RouteMode } from './route-mode.model'; 

export class PhotoListContext {
    private _lastGpsIndex: number = null;
    private _firstGpsIndex: number = null;
    private _intervalId: number = null;
    current: Photo = null;
    currentIndex: number = -1;
    photoUpdated: EventEmitter<number> = new EventEmitter<number>();

    constructor(public photos: Array<Photo>,
        private _routeMode: RouteMode,
        private _stateService: PhotoStateService) {
        if (photos == null) {
            throw new RangeError('photos cannot be null');
        }

        this.updateGpsIndices();
    }

    get isCategoryMode(): boolean {
        return this._routeMode === RouteMode.Category;
    }

    get isRandomMode(): boolean {
        return this._routeMode === RouteMode.Random;
    }

    get hasNext(): boolean {
        return this.currentIndex < this.photos.length - 1;
    }

    get hasPrevious(): boolean {
        return this.currentIndex > 0;
    }

    get hasGpsNext(): boolean {
        return this._lastGpsIndex != null && this.currentIndex < this._lastGpsIndex;
    }

    get hasGpsPrevious(): boolean {
        return this._firstGpsIndex != null && this.currentIndex > this._firstGpsIndex;
    }

    get isSlideshowPlaying(): boolean {
        return this._intervalId != null;
    }

    moveNext(): void {
        if (this.hasNext) {
            this.moveTo(this.currentIndex + 1);
        }
    }

    movePrevious(): void {
        if (this.hasPrevious) {
            this.moveTo(this.currentIndex - 1);
        }
    }

    moveGpsNext(): void {
        if (this.hasGpsNext) {
            for (let i = this.currentIndex + 1; i <= this._lastGpsIndex; i++) {
                if (this.photos[i].hasGps) {
                    this.moveTo(i);
                    break;
                }
            }
        }
    }

    moveGpsPrevious(): void {
        if (this.hasGpsPrevious) {
            for (let i = this.currentIndex - 1; i >= 0; i--) {
                if (this.photos[i].hasGps) {
                    this.moveTo(i);
                    break;
                }
            }
        }
    }

    moveToPhoto(photo: Photo): void {
        for (let i = 0; i < this.photos.length; i++) {
            if (this.photos[i].photo.id === photo.photo.id) {
                this.moveTo(i);
                return;
            }
        }
    }

    moveTo(newIndex: number): void {
        if (newIndex !== this.currentIndex) {
            this.currentIndex = newIndex;
            this.current = this.photos[this.currentIndex];

            this.photoUpdated.next(newIndex);
        }
    }

    toggleSlideshow(): void {
        if (!this.isSlideshowPlaying) {
            this.tickSlideshow();
            this._intervalId = setInterval(() => this.tickSlideshow(), this._stateService.config.slideshowIntervalSeconds * 1000);
        }
        else {
            this.terminateSlideshow();
        }
    }

    getPhotosWithGpsData(): Array<Photo> {
        return this.photos.filter(x => x.hasGps);
    }

    terminateSlideshow() {
        if (this.isSlideshowPlaying) {
            clearInterval(this._intervalId);
            this._intervalId = null;
        }
    }

    private tickSlideshow(): void {
        if (this.hasNext) {
            this.moveNext();
        }
        else {
            this.terminateSlideshow();
        }
    }

    private updateGpsIndices() {
        this._firstGpsIndex = null;
        this._lastGpsIndex = null;

        for (let i = 0; i < this.photos.length; i++) {
            if (this.photos[i].hasGps) {
                this._firstGpsIndex = i;
                break;
            }
        }

        if (this._firstGpsIndex != null) {
            for (let i = this.photos.length - 1; i >= 0; i--) {
                if (this.photos[i].hasGps) {
                    this._lastGpsIndex = i;
                    break;
                }
            }
        }
    }
}
