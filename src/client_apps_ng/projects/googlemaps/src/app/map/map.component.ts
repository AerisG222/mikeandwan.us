import { Component, Output, EventEmitter, Input, ViewChild } from '@angular/core';
import { GoogleMap } from '@angular/google-maps';

@Component({
    selector: 'app-map',
    templateUrl: './map.component.html',
    styleUrls: [ './map.component.scss' ]
})
export class MapComponent {
    @ViewChild(GoogleMap) map?: GoogleMap;

    @Input() poi?: google.maps.LatLng;
    @Input() center?: google.maps.LatLng;
    @Input() zoom?: number;

    @Output() centerChanged = new EventEmitter<google.maps.LatLng>();
    @Output() boundsChanged = new EventEmitter<google.maps.LatLngBounds>();

    options?: google.maps.MapOptions;
    showRectangle = false;
    rectangleBounds?: google.maps.LatLngBounds;

    get otherMapBounds(): google.maps.LatLngBounds | undefined { return this.rectangleBounds; }

    @Input()
    set otherMapBounds(bounds: google.maps.LatLngBounds | undefined) {
        this.rectangleBounds = bounds;

        if (this.map)
        {
            const ourBounds = this.map.getBounds();

            if (ourBounds && bounds) {
                const ourSize = Math.abs(ourBounds.getSouthWest().lat() - ourBounds.getNorthEast().lat());
                const theirSize = Math.abs(bounds.getSouthWest().lat() - bounds.getNorthEast().lat());

                this.showRectangle = ourSize > theirSize;
            }
        }
    }

    onBoundsChanged(): void {
        if (this.map)
        {
            const bounds = this.map.getBounds();

            if (bounds)
            {
                this.boundsChanged.emit(bounds);
            }
        }
    }

    onCenterChanged(): void {
        if (this.map)
        {
            this.centerChanged.emit(this.map.getCenter());
        }
    }
}
