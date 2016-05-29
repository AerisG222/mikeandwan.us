import { Component, Input } from '@angular/core';
import { NgFor } from '@angular/common';
import { PhotoDataService, ExifDetail, ExifFormatter, Photo } from '../shared';

@Component({
    moduleId: module.id,
    selector: 'app-exif-view',
    directives: [ NgFor ],
    templateUrl: 'exif-view.component.html',
    styleUrls: ['exif-view.component.css']
})
export class ExifViewComponent {
    private _photo: Photo;
    private _map: Array<ExifFormatter> = [
        new ExifFormatter("AF Point", "afPoint", null),
        new ExifFormatter("Aperture", "aperture", null),
        new ExifFormatter("Contrast", "contrast", null),
        new ExifFormatter("Depth Of Field", "depthOfField", null),
        new ExifFormatter("Digital Zoom Ratio", "digitalZoomRatio", null),
        new ExifFormatter("Exposure Compensation", "exposureCompensation", null),
        new ExifFormatter("Exposure Difference", "exposureDifference", null),
        new ExifFormatter("Exposure Mode", "exposureMode", null),
        new ExifFormatter("Exposure Time", "exposureTime", this.inverseTime),
        new ExifFormatter("F Number", "fNumber", null),
        new ExifFormatter("Flash", "flash", null),
        new ExifFormatter("Flash Exposure Compensation", "flashExposureCompensation", null),
        new ExifFormatter("Flash Mode", "flashMode", null),
        new ExifFormatter("Flash Setting", "flashSetting", null),
        new ExifFormatter("Flash Type", "flashType", null),
        new ExifFormatter("Focal Length", "focalLength", this.focalLengthOneDecimal),
        new ExifFormatter("Focal Length in 35mm Format", "focalLengthIn35mmFormat", this.focalLengthNoDecimals),
        new ExifFormatter("Focus Distance", "focusDistance", this.distance),
        new ExifFormatter("Focus Mode", "focusMode", null),
        new ExifFormatter("Focus Position", "focusPosition", null),
        new ExifFormatter("Gain Control", "gainControl", null),
        new ExifFormatter("Hue Adjustment", "hueAdjustment", null),
        new ExifFormatter("Hyperfocal Distance", "hyperFocalDistance", this.distance),
        new ExifFormatter("ISO", "iso", null),
        new ExifFormatter("Lens ID", "lensId", null),
        new ExifFormatter("Light Source", "lightSource", null),
        new ExifFormatter("Make", "make", null),
        new ExifFormatter("Metering Mode", "meteringMode", null),
        new ExifFormatter("Model", "model", null),
        new ExifFormatter("Noise Reduction", "noiseReduction", null),
        new ExifFormatter("Orientation", "orientation", null),
        new ExifFormatter("Saturation", "saturation", null),
        new ExifFormatter("Scale Factor 35 EFL", "scaleFactor35Efl", this.oneDecimal),
        new ExifFormatter("Scene Capture Type", "sceneCaptureType", null),
        new ExifFormatter("Scene Type", "sceneType", null),
        new ExifFormatter("Sensing Method", "sensingMethod", null),
        new ExifFormatter("Sharpness", "sharpness", null),
        new ExifFormatter("Shutter Speed", "shutterSpeed", this.inverseTime),
        new ExifFormatter("White Balance", "whiteBalance", null),
        new ExifFormatter("Shot Taken Date", "shotTakenDate", null),
        new ExifFormatter("Exposure Program", "exposureProgram", null),
        new ExifFormatter("GPS Version ID", "gpsVersionId", null),
        new ExifFormatter("GPS Latitude", "gpsLatitude", this.formatLatitude),
        new ExifFormatter("GPS Longitude", "gpsLongitude", this.formatLongitude),
        new ExifFormatter("GPS Altitude", "gpsAltitude", this.formatAltitude),
        new ExifFormatter("GPS Time Stamp", "gpsTime", null),
        new ExifFormatter("GPS Satellites", "gpsSatellites", null)];

    exif: Array<Array<ExifDetail>> = [];

    @Input() set photo(value: Photo) {
        this._photo = value;
        
        if(this._photo.exif) {
            this.exif = this._photo.exif;
        }
        else {
            this.getExifData();
        }
    }

    get photo(): Photo {
        return this._photo;
    }
    
    constructor(private _dataService: PhotoDataService) {

    }

    formatExif(val: any, formatFunc: Function): string {
        if (val == null || val === '') {
            return '--';
        }

        if (formatFunc !== null) {
            return formatFunc(<number>val);
        }

        return val.toString();
    }

    inverseTime(val: number): string {
        if (val >= 1) {
            return val.toString();
        }

        return `1/${Math.round(1.0 / val)}`;
    }

    focalLengthOneDecimal(val: number): string {
        return `${val.toFixed(1)} mm`;
    }

    focalLengthNoDecimals(val: number): string {
        return `${val} mm`;
    }

    oneDecimal(val: number): string {
        return val.toFixed(1);
    }

    distance(val: number): string {
        return `${val.toFixed(2)} m`;
    }

    formatLatitude(val: number): string {
        if (val >= 0) {
            return `${val} (North)`;
        }
        else {
            return `${val} (South)`;
        }
    }

    formatLongitude(val : number): string {
        if (val >= 0) {
            return `${val} (East)`;
        }
        else {
            return `${val} (West)`;
        }
    }

    formatAltitude(val : number): string {
        if (val >= 0) {
            return `${val} m Above Sea Level`;
        }
        else {
            return `${val} m Below Sea Level`;
        }
    }

    private getExifData(): void {
        this._dataService.getPhotoExifData(this._photo.photo.id)
            .subscribe(exif => {
                let detail : Array<Array<ExifDetail>> = [];

                for (let i = 0; i < this._map.length; i++) {
                    let fmt = this._map[i];
                    let item1 = new ExifDetail(fmt.displayName, this.formatExif((<any>exif)[fmt.fieldName], fmt.formatFunction));
                    let item2 = new ExifDetail('', '');

                    i++;

                    if (i < this._map.length) {
                        fmt = this._map[i];
                        item2 = new ExifDetail(fmt.displayName, this.formatExif((<any>exif)[fmt.fieldName], fmt.formatFunction));
                    }

                    detail.push([item1, item2]);
                }

                this.photo.exif = detail;
                this.exif = detail;
            });
    }
}
