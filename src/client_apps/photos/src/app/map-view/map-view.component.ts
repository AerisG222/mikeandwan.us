import { Component, Input, ElementRef, ChangeDetectorRef, OnDestroy, AfterViewInit } from '@angular/core';

import { Photo } from '../shared/photo.model';
import { IPhoto } from '../shared/iphoto.model';
import { PhotoListContext } from '../shared/photo-list-context.model';

@Component({
    selector: 'app-map-view',
    templateUrl: 'map-view.component.html',
    styleUrls: [ 'map-view.component.css' ]
})
export class MapViewComponent implements OnDestroy, AfterViewInit {
    private _markerImage = {
        url: '/css/photo_app/map_marker.png',
        size: new google.maps.Size(11, 11),
        origin: new google.maps.Point(0, 0),
        anchor: new google.maps.Point(5, 11)
    };
    private _photos: Array<Photo>;
    private _mapDiv: HTMLDivElement;
    private _mapButtonsDiv: HTMLDivElement;
    private _googleMap: google.maps.Map = null;
    private _googleMapMarkerList: Array<google.maps.Marker> = [];
    private _openInfoWindow: google.maps.InfoWindow = null;
    @Input() context: PhotoListContext;

    constructor(private _changeDetectionRef: ChangeDetectorRef,
                private _elRef: ElementRef) {

    }

    ngAfterViewInit(): void {
        this._mapDiv = this._elRef.nativeElement.getElementsByClassName('map')[0];
        this._mapButtonsDiv = this._elRef.nativeElement.getElementsByClassName('mapButtons')[0];

        this.context.photoUpdated.subscribe(() => {
            this.onPhotoUpdated();
        });

        this._photos = this.context.getPhotosWithGpsData();
        this.initMap();
        this.addPhotosToMap();

        Mousetrap.bind('right', () => this.runAndCheck(() => this.context.moveGpsNext()));
        Mousetrap.bind('left', () => this.runAndCheck(() => this.context.moveGpsPrevious()));
    }

    ngOnDestroy(): void {
        Mousetrap.reset();
    }

    onMapMarkerClicked(marker: google.maps.Marker, infoWindow: google.maps.InfoWindow, photo: Photo, suppressEvent: boolean): void {
        if (this._openInfoWindow !== null) {
            this._openInfoWindow.close();
        }

        this._openInfoWindow = infoWindow;
        infoWindow.open(this._googleMap, marker);

        this.context.moveToPhoto(photo);
    }

    movePrev(): void {
        this.context.moveGpsPrevious();
    }

    moveNext(): void {
        this.context.moveGpsNext();
    }

    private onPhotoUpdated(): void {
        let index = -1;
        let photo = this.context.photos[this.context.currentIndex];

        for (let i = 0; i < this._photos.length; i++) {
            if (this._photos[i].photo.id === photo.photo.id) {
                index = i;
                break;
            }
        }

        if (index >= 0) {
            // here we 'click' the marker to get the infowindow to appear - but we pass the last argument
            // as we don't want to continually fire this event!
            google.maps.event.trigger(this._googleMapMarkerList[index], 'click', true);
        }
    }

    private initMap(): void {
        let center = new google.maps.LatLng(this._photos[0].photo.latitude, this._photos[0].photo.longitude);

        let opts = {
            center: center,
            zoom: 17,
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            navigationControlOptions: { style: google.maps.MapTypeControlStyle.DEFAULT }
        };

        this._googleMap = new google.maps.Map(this._mapDiv, opts);
        this._googleMap.controls[google.maps.ControlPosition.TOP_RIGHT].push(this._mapButtonsDiv);
    }

    private addPhotosToMap(): void {
        for (let i = 0; i < this._photos.length; i++) {
            let photo = this._photos[i];
            let marker = this.createMapMarker(photo);

            this._googleMapMarkerList.push(marker);
        }
    }

    private generatePhotoHtmlForMap(photo: IPhoto): string {
        return `<img src="${photo.xsInfo.path}" class="img-thumbnail" />`;
    }

    private createMapMarker(photo: Photo): google.maps.Marker {
        let point = new google.maps.LatLng(photo.photo.latitude, photo.photo.longitude);
        let infoWindow = new google.maps.InfoWindow({ content: this.generatePhotoHtmlForMap(photo.photo) });
        let marker = new google.maps.Marker({
            icon: this._markerImage,
            position: point,
            map: this._googleMap
        });

        google.maps.event.addListener(marker, 'click', (suppressEvent: boolean) => {
            this.onMapMarkerClicked(marker, infoWindow, photo, suppressEvent);
        });

        return marker;
    }

    private runAndCheck(func: Function) {
        func();
        this._changeDetectionRef.detectChanges();
    }
}
