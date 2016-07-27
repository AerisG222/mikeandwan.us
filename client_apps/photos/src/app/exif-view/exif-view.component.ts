import { Component, Input } from '@angular/core';
import { NgFor } from '@angular/common';

import { PhotoDataService } from '../shared/photo-data.service';
import { ExifDetail } from '../shared/exif-detail.model';
import { ExifFormatter } from '../shared/exif-formatter.model';
import { Photo } from '../shared/photo.model';

@Component({
    moduleId: module.id,
    selector: 'exif-view',
    directives: [ NgFor ],
    templateUrl: 'exif-view.component.html',
    styleUrls: [ 'exif-view.component.css' ]
})
export class ExifViewComponent {
    private _photo: Photo;
    private _map: Array<ExifFormatter> = [
        new ExifFormatter('Bits per Sample', 'bitsPerSample', null),
        new ExifFormatter('Compression', 'compression', null),
        new ExifFormatter('Contrast', 'contrast', null),
        new ExifFormatter('Create Date', 'createDate', null),
        new ExifFormatter('Digital Zoom Ratio', 'digitalZoomRatio', null),
        new ExifFormatter('Exposure Compensation', 'exposureCompensation', null),
        new ExifFormatter('Exposure Mode', 'exposureMode', null),
        new ExifFormatter('Exposure Program', 'exposureProgram', null),
        new ExifFormatter('Exposure Time', 'exposureTime', null),
        new ExifFormatter('F Number', 'fNumber', null),
        new ExifFormatter('Flash', 'flash', null),
        new ExifFormatter('Focal Length', 'focalLength', null),
        new ExifFormatter('Focal Length in 35mm Format', 'focalLengthIn35mmFormat', null),
        new ExifFormatter('Gain Control', 'gainControl', null),
        new ExifFormatter('GPS Altitude', 'gpsAltitude', this.formatAltitude),
        new ExifFormatter('GPS Time Stamp', 'gpsDateStamp', null),
        new ExifFormatter('GPS Direction', 'gpsDirection', null),
        new ExifFormatter('GPS Latitude', 'gpsLatitude', this.formatLatitude),
        new ExifFormatter('GPS Longitude', 'gpsLongitude', this.formatLongitude),
        new ExifFormatter('GPS Measure Mode', 'gpsMeasureMode', null),
        new ExifFormatter('GPS Satellites', 'gpsSatellites', null),
        new ExifFormatter('GPS Status', 'gpsStatus', null),
        new ExifFormatter('GPS Version ID', 'gpsVersionId', null),
        new ExifFormatter('ISO', 'iso', null),
        new ExifFormatter('Light Source', 'lightSource', null),
        new ExifFormatter('Make', 'make', null),
        new ExifFormatter('Metering Mode', 'meteringMode', null),
        new ExifFormatter('Model', 'model', null),
        new ExifFormatter('Orientation', 'orientation', null),
        new ExifFormatter('Saturation', 'saturation', null),
        new ExifFormatter('Scene Capture Type', 'sceneCaptureType', null),
        new ExifFormatter('Scene Type', 'sceneType', null),
        new ExifFormatter('Sensing Method', 'sensingMethod', null),
        new ExifFormatter('Sharpness', 'sharpness', null),
        new ExifFormatter('Auto Focus Area Mode', 'autoFocusAreaMode', null),
        new ExifFormatter('Auto Focus Point', 'autoFocusPoint', null),
        new ExifFormatter('Active D Lighting', 'activeDLighting', null),
        new ExifFormatter('Colorspace', 'colorspace', null),
        new ExifFormatter('Exposure Difference', 'exposureDifference', null),
        new ExifFormatter('Flash Color Filter', 'flashColorFilter', null),
        new ExifFormatter('Flash Compensation', 'flashCompensation', null),
        new ExifFormatter('Flash Control Mode', 'flashControlMode', null),
        new ExifFormatter('Flash Exposure Compensation', 'flashExposureCompensation', null),
        new ExifFormatter('Flash Focal Length', 'flashFocalLength', null),
        new ExifFormatter('Flash Mode', 'flashMode', null),
        new ExifFormatter('Flash Setting', 'flashSetting', null),
        new ExifFormatter('Flash Type', 'flashType', null),
        new ExifFormatter('Focus Distance', 'focusDistance', this.distance),
        new ExifFormatter('Focus Mode', 'focusMode', null),
        new ExifFormatter('Focus Position', 'focusPosition', null),
        new ExifFormatter('High ISO Noise Reduction', 'highIsoNoiseReduction', null),
        new ExifFormatter('Hue Adjustment', 'hueAdjustment', null),
        new ExifFormatter('Noise Reduction', 'noiseReduction', null),
        new ExifFormatter('Picture Control', 'pictureControlName', null),
        new ExifFormatter('Primary AF Point', 'primaryAFPoint', null),
        new ExifFormatter('VR Mode', 'vrMode', null),
        new ExifFormatter('Vibration Reduction', 'vibrationReduction', null),
        new ExifFormatter('Vignette Control', 'vignetteControl', null),
        new ExifFormatter('White Balance', 'whiteBalance', null),
        new ExifFormatter('Aperture', 'aperture', null),
        new ExifFormatter('Auto Focus', 'autoFocus', null),
        new ExifFormatter('Depth Of Field', 'depthOfField', null),
        new ExifFormatter('Field of View', 'fieldOfView', null),
        new ExifFormatter('Hyperfocal Distance', 'hyperfocalDistance', this.distance),
        new ExifFormatter('Lens ID', 'lensId', null),
        new ExifFormatter('Light Value', 'lightValue', this.fourDecimals),
        new ExifFormatter('Scale Factor 35 EFL', 'scaleFactor35Efl', null),
        new ExifFormatter('Shutter Speed', 'shutterSpeed', null),
        new ExifFormatter('Raw Conversion Mode', 'rawConversionMode', null),
        new ExifFormatter('Sigmoidal Contrast Adjustment', 'sigmoidalContrastAdjustment', this.fourDecimals),
        new ExifFormatter('Saturation Adjustment', 'saturationAdjustment', this.fourDecimals),
        new ExifFormatter('Compression Quality', 'compressionQuality', null),
    ];

    exif: Array<Array<ExifDetail>> = [];

    @Input() set photo(value: Photo) {
        this._photo = value;

        if (this._photo.exif) {
            this.exif = this._photo.exif;
        } else {
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

    fourDecimals(val: number): string {
        return val.toFixed(4);
    }

    distance(val: number): string {
        return `${val.toFixed(2)} m`;
    }

    formatLatitude(val: number): string {
        if (val >= 0) {
            return `${val} (North)`;
        } else {
            return `${val} (South)`;
        }
    }

    formatLongitude(val: number): string {
        if (val >= 0) {
            return `${val} (East)`;
        } else {
            return `${val} (West)`;
        }
    }

    formatAltitude(val: number): string {
        if (val >= 0) {
            return `${val} m Above Sea Level`;
        } else {
            return `${val} m Below Sea Level`;
        }
    }

    private getExifData(): void {
        this._dataService.getPhotoExifData(this._photo.photo.id)
            .subscribe(exif => {
                let detail: Array<Array<ExifDetail>> = [];

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