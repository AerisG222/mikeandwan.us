import { EventEmitter } from '@angular/core';

import { PhotoListContext, PhotoStateService, RouteMode, Photo, RandomPhotoSource } from './';

export class RandomPhotoListContext extends PhotoListContext {
    photoAddedEventEmitter = new EventEmitter<Photo>();

    constructor(photos: Array<Photo>,
        routeMode: RouteMode,
        stateService: PhotoStateService,
        private _photoSource: RandomPhotoSource) {
        super(photos, routeMode, stateService);
    }

    get hasNext(): boolean {
        return true;
    }

    moveNext(): void {
        this._photoSource.getPhotos().subscribe((photos: Array<Photo>) => {
            this.photos.push(photos[0]);
            this.photoAddedEventEmitter.next(photos[0]);
            super.moveNext();
        });
    }
}
