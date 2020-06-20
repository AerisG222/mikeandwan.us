import { Component, NgZone } from '@angular/core';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: [ './app.component.scss' ]
})
export class AppComponent {
    addressToMap = '';
    showMaps = false;
    poi: google.maps.LatLng;
    center: google.maps.LatLng;
    geocoder: google.maps.Geocoder = new google.maps.Geocoder();
    map1Bounds: google.maps.LatLngBounds;
    map2Bounds: google.maps.LatLngBounds;

    constructor(private zone: NgZone) {

    }

    showSampleAddress(evt: Event, address: string): void {
        evt.preventDefault();
        this.addressToMap = address;
        this.showAddress();
    }

    showAddress(): void {
        this.geocoder.geocode({ address: this.addressToMap }, (results, status) => {
            this.zone.run(() => {
                if (status !== google.maps.GeocoderStatus.OK) {
                    alert(`There was an error geocoding the address: ${this.addressToMap}].  Reported error code = ${status}`);
                } else {
                    this.poi = results[0].geometry.location;
                    this.center = this.poi;
                    this.showMaps = true;
                }
            });
        });
    }

    onMap1BoundsChanged(bounds: google.maps.LatLngBounds) {
        this.map1Bounds = bounds;
    }

    onMap2BoundsChanged(bounds: google.maps.LatLngBounds) {
        this.map2Bounds = bounds;
    }

    onCenterChanged(center: google.maps.LatLng) {
        this.center = center;
    }
}
