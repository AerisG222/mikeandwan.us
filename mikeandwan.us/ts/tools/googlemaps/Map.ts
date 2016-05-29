import { Component, Output, ElementRef, EventEmitter, AfterViewInit } from '@angular/core';
import { BoundingBox } from './BoundingBox';
import { MapContext } from './MapContext';

@Component({
    selector: 'map',
    templateUrl: '/js/tools/googlemaps/Map.html'
})
export class Map implements AfterViewInit {
	isVisible : boolean = false;
	map : google.maps.Map = null;
    mapDiv : HTMLElement;
	marker : google.maps.Marker = null;
	mapMoveListener : google.maps.MapsEventListener = null;
    mapZoomListener : google.maps.MapsEventListener = null;
	boundingBoxOverlay : google.maps.Polyline = null;
	@Output() updated : EventEmitter<MapContext> = new EventEmitter<MapContext>();
	@Output() loaded : EventEmitter<any> = new EventEmitter<any>();
    
	constructor(private _elRef: ElementRef) {
		
	}
    
    ngAfterViewInit() : void {
        this.mapDiv = this._elRef.nativeElement.getElementsByClassName('map')[0];
    }
    
	show() : void {
		this.isVisible = true;
		
		let myOptions = {
			zoom: 8,
			mapTypeId: google.maps.MapTypeId.ROADMAP,
			navigationControlOptions: { style: google.maps.MapTypeControlStyle.DEFAULT }
		};
		
		this.map = new google.maps.Map(this.mapDiv, myOptions);
        
        google.maps.event.addListenerOnce(this.map, 'idle', () => {
            this.loaded.next(null);
        });
	}
	
	hide() : void {
		if(this.isVisible) {
			this.isVisible = false;
			this.disableMapEvents();
			this.map = null;	
		}
	}
	
	enableMapEvents() : void {
		if(this.mapMoveListener === null && this.map !== null) {
			this.mapMoveListener = google.maps.event.addListener(this.map, 'center_changed', (...args: any[]) => { this.mapUpdated(); });
			this.mapZoomListener = google.maps.event.addListener(this.map, 'zoom_changed', (...args: any[]) => { this.mapUpdated(); });
		}
	}
	
	disableMapEvents() : void {
		if(this.mapMoveListener !== null) {
			google.maps.event.removeListener(this.mapMoveListener);
			google.maps.event.removeListener(this.mapZoomListener);
	
			this.mapMoveListener = null;
			this.mapZoomListener = null;
		}
	}
	
	mapUpdated() : void {
		this.updated.next(new MapContext(this.map.getZoom(), this.map.getCenter(), new BoundingBox(this.map)));
	}
	
	syncMap(ctx : MapContext) : void {
		this.map.panTo(ctx.center);
		this.drawOverlayBounds(ctx);
	}
	
	setViewCoordinates(zoom : number, centerLatLng : google.maps.LatLng) : void {
		this.map.setCenter(centerLatLng);
		this.map.setZoom(zoom);
	
		if(this.marker !== null) {
			this.marker.setMap(null);
			this.marker = null;
		}
	
		this.marker = new google.maps.Marker({ position: centerLatLng, map: this.map });
	}
	
	drawOverlayBounds(ctx : MapContext) : void {
		// make sure the existing overlay is gonzo
		if(this.boundingBoxOverlay !== null) {
			this.boundingBoxOverlay.setMap(null);
			this.boundingBoxOverlay = null;
		}
	
		// only try to draw overlay when the second is at greater zoom
		if(ctx.zoom > this.map.getZoom()) {
			this.displayBoundingBox(ctx);	
		}
	}

	displayBoundingBox(ctx : MapContext) : void {
		if(ctx.boundingBox !== null) {
			this.boundingBoxOverlay = new google.maps.Polyline({ path: ctx.boundingBox.boundsArray,
				   											     strokeColor: '#cc0000', 
															     strokeWeight: 2, 
															     strokeOpacity: 0.8 });
	
			this.boundingBoxOverlay.setMap(this.map);
		}
	}
}
