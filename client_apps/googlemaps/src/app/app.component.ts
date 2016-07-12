import { Component, ViewChild, ChangeDetectorRef } from '@angular/core';
import { NgIf } from '@angular/common';
import 'rxjs/add/operator/take';
import 'rxjs/add/operator/concat';

import { MapComponent } from './map/map.component';

@Component({
    moduleId: module.id,
    selector: 'googlemaps-app',
    directives: [ NgIf, MapComponent ],
    templateUrl: 'app.component.html',
    styleUrls: [ 'app.component.css' ]
})
export class GooglemapsAppComponent {
    show: boolean = true;
    geocoder: google.maps.Geocoder = new google.maps.Geocoder();

    @ViewChild('map1') map1: MapComponent;
    @ViewChild('map2') map2: MapComponent;

    constructor(private _changeDetectionRef: ChangeDetectorRef) {

    }

    showMaps(loc: google.maps.LatLng): void {
        this.map1.show(17, loc);
        this.map2.show(13, loc);
        this._changeDetectionRef.detectChanges();
    }

    hideMaps(): void {
        this.map1.hide();
        this.map2.hide();
        this._changeDetectionRef.detectChanges();
    }

    showAddress(address: string): void {
        this.geocoder.geocode({ address: address }, (results, status) => {
            if (status !== google.maps.GeocoderStatus.OK) {
                this.hideMaps();
                alert(`There was an error geocoding the address: ${address}].  Reported error code = ${status}`);
            } else {
                this.showMaps(results[0].geometry.location);
            }
        });
    }
}
