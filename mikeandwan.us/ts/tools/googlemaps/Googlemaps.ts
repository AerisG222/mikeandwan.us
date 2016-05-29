import { Component, ViewChild, ChangeDetectorRef } from '@angular/core';
import { NgIf } from '@angular/common';
import { Map } from './Map';
import 'rxjs/add/operator/take';
import 'rxjs/add/operator/concat';

@Component({
    selector: 'googlemaps',
    directives: [ NgIf, Map ],
    templateUrl: '/js/tools/googlemaps/Googlemaps.html'
})
export class Googlemaps {
	show : boolean = true;
    geocoder : google.maps.Geocoder= new google.maps.Geocoder();
	@ViewChild('map1') map1 : Map;
	@ViewChild('map2') map2 : Map;
	
	constructor(private _changeDetectionRef : ChangeDetectorRef) {
		
	}
	
    showMaps(loc : google.maps.LatLng) : void {
        let m1 = this.map1.loaded.take(1);
        let m2 = this.map2.loaded.take(1);
        
        // wait for both maps to load before updating view coords
        m1.concat(m2).subscribe(() => {
            this.setViewCoordinates(loc);
        });
        
        this.map1.show();
        this.map2.show();
        this._changeDetectionRef.detectChanges();
	}

	hideMaps() : void {
        this.map1.hide();
        this.map2.hide();
        this._changeDetectionRef.detectChanges();
	}

	showAddress(address : string) : void {
		this.geocoder.geocode({ address: address }, (results, status) => {
			if (status !== google.maps.GeocoderStatus.OK) { 
				this.hideMaps();
				alert(`There was an error geocoding the address: ${address}].  Reported error code = ${status}`);
			} 
			else {
				this.showMaps(results[0].geometry.location);
			}
		});
	}

	setViewCoordinates(latLng : google.maps.LatLng) : void {
		this.map1.setViewCoordinates(17, latLng);
		this.map2.setViewCoordinates(15, latLng);
		
		// force both maps to flag an update
		this.map1.mapUpdated();
		this.map2.mapUpdated();
	}
}
