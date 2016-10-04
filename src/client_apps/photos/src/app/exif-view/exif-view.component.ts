import { Component, Input } from '@angular/core';

import { ExifCategory } from './exif-category.model';
import { PhotoDataService } from '../shared/photo-data.service';
import { ExifDetail } from './exif-detail.model';
import { ExifFormatter } from './exif-formatter.model';
import { Photo } from '../shared/photo.model';
import { PhotoExifInfo } from '../shared/photo-exif-info.model';

@Component({
    selector: 'exif-view',
    templateUrl: 'exif-view.component.html',
    styleUrls: [ 'exif-view.component.css' ]
})
export class ExifViewComponent {
    private _photo: Photo;
    private _map: Array<ExifFormatter> = [
        new ExifFormatter(ExifCategory.Exif, 'Bits per Sample', 'bitsPerSample', null),
        new ExifFormatter(ExifCategory.Exif, 'Compression', 'compression', null),
        new ExifFormatter(ExifCategory.Exif, 'Contrast', 'contrast', null),
        new ExifFormatter(ExifCategory.Exif, 'Create Date', 'createDate', null),
        new ExifFormatter(ExifCategory.Exif, 'Digital Zoom Ratio', 'digitalZoomRatio', null),
        new ExifFormatter(ExifCategory.Exif, 'Exposure Compensation', 'exposureCompensation', null),
        new ExifFormatter(ExifCategory.Exif, 'Exposure Mode', 'exposureMode', null),
        new ExifFormatter(ExifCategory.Exif, 'Exposure Program', 'exposureProgram', null),
        new ExifFormatter(ExifCategory.Exif, 'Exposure Time', 'exposureTime', null),
        new ExifFormatter(ExifCategory.Exif, 'F Number', 'fNumber', null),
        new ExifFormatter(ExifCategory.Exif, 'Flash', 'flash', null),
        new ExifFormatter(ExifCategory.Exif, 'Focal Length', 'focalLength', null),
        new ExifFormatter(ExifCategory.Exif, 'Focal Length in 35mm Format', 'focalLengthIn35mmFormat', null),
        new ExifFormatter(ExifCategory.Exif, 'Gain Control', 'gainControl', null),
        new ExifFormatter(ExifCategory.Exif, 'GPS Altitude', 'gpsAltitude', this.formatAltitude),
        new ExifFormatter(ExifCategory.Exif, 'GPS Time Stamp', 'gpsDateStamp', null),
        new ExifFormatter(ExifCategory.Exif, 'GPS Direction', 'gpsDirection', null),
        new ExifFormatter(ExifCategory.Exif, 'GPS Latitude', 'gpsLatitude', this.formatLatitude),
        new ExifFormatter(ExifCategory.Exif, 'GPS Longitude', 'gpsLongitude', this.formatLongitude),
        new ExifFormatter(ExifCategory.Exif, 'GPS Measure Mode', 'gpsMeasureMode', null),
        new ExifFormatter(ExifCategory.Exif, 'GPS Satellites', 'gpsSatellites', null),
        new ExifFormatter(ExifCategory.Exif, 'GPS Status', 'gpsStatus', null),
        new ExifFormatter(ExifCategory.Exif, 'GPS Version ID', 'gpsVersionId', null),
        new ExifFormatter(ExifCategory.Exif, 'ISO', 'iso', null),
        new ExifFormatter(ExifCategory.Exif, 'Light Source', 'lightSource', null),
        new ExifFormatter(ExifCategory.Exif, 'Make', 'make', null),
        new ExifFormatter(ExifCategory.Exif, 'Metering Mode', 'meteringMode', null),
        new ExifFormatter(ExifCategory.Exif, 'Model', 'model', null),
        new ExifFormatter(ExifCategory.Exif, 'Orientation', 'orientation', null),
        new ExifFormatter(ExifCategory.Exif, 'Saturation', 'saturation', null),
        new ExifFormatter(ExifCategory.Exif, 'Scene Capture Type', 'sceneCaptureType', null),
        new ExifFormatter(ExifCategory.Exif, 'Scene Type', 'sceneType', null),
        new ExifFormatter(ExifCategory.Exif, 'Sensing Method', 'sensingMethod', null),
        new ExifFormatter(ExifCategory.Exif, 'Sharpness', 'sharpness', null),
        new ExifFormatter(ExifCategory.Maker, 'Auto Focus Area Mode', 'autoFocusAreaMode', null),
        new ExifFormatter(ExifCategory.Maker, 'Auto Focus Point', 'autoFocusPoint', null),
        new ExifFormatter(ExifCategory.Maker, 'Active D Lighting', 'activeDLighting', null),
        new ExifFormatter(ExifCategory.Maker, 'Colorspace', 'colorspace', null),
        new ExifFormatter(ExifCategory.Maker, 'Exposure Difference', 'exposureDifference', null),
        new ExifFormatter(ExifCategory.Maker, 'Flash Color Filter', 'flashColorFilter', null),
        new ExifFormatter(ExifCategory.Maker, 'Flash Compensation', 'flashCompensation', null),
        new ExifFormatter(ExifCategory.Maker, 'Flash Control Mode', 'flashControlMode', null),
        new ExifFormatter(ExifCategory.Maker, 'Flash Exposure Compensation', 'flashExposureCompensation', null),
        new ExifFormatter(ExifCategory.Maker, 'Flash Focal Length', 'flashFocalLength', null),
        new ExifFormatter(ExifCategory.Maker, 'Flash Mode', 'flashMode', null),
        new ExifFormatter(ExifCategory.Maker, 'Flash Setting', 'flashSetting', null),
        new ExifFormatter(ExifCategory.Maker, 'Flash Type', 'flashType', null),
        new ExifFormatter(ExifCategory.Maker, 'Focus Distance', 'focusDistance', this.distance),
        new ExifFormatter(ExifCategory.Maker, 'Focus Mode', 'focusMode', null),
        new ExifFormatter(ExifCategory.Maker, 'Focus Position', 'focusPosition', null),
        new ExifFormatter(ExifCategory.Maker, 'High ISO Noise Reduction', 'highIsoNoiseReduction', null),
        new ExifFormatter(ExifCategory.Maker, 'Hue Adjustment', 'hueAdjustment', null),
        new ExifFormatter(ExifCategory.Maker, 'Noise Reduction', 'noiseReduction', null),
        new ExifFormatter(ExifCategory.Maker, 'Picture Control', 'pictureControlName', null),
        new ExifFormatter(ExifCategory.Maker, 'Primary AF Point', 'primaryAFPoint', null),
        new ExifFormatter(ExifCategory.Maker, 'VR Mode', 'vrMode', null),
        new ExifFormatter(ExifCategory.Maker, 'Vibration Reduction', 'vibrationReduction', null),
        new ExifFormatter(ExifCategory.Maker, 'Vignette Control', 'vignetteControl', null),
        new ExifFormatter(ExifCategory.Maker, 'White Balance', 'whiteBalance', null),
        new ExifFormatter(ExifCategory.Composite, 'Aperture', 'aperture', null),
        new ExifFormatter(ExifCategory.Composite, 'Auto Focus', 'autoFocus', null),
        new ExifFormatter(ExifCategory.Composite, 'Depth Of Field', 'depthOfField', null),
        new ExifFormatter(ExifCategory.Composite, 'Field of View', 'fieldOfView', null),
        new ExifFormatter(ExifCategory.Composite, 'Hyperfocal Distance', 'hyperfocalDistance', this.distance),
        new ExifFormatter(ExifCategory.Composite, 'Lens ID', 'lensId', null),
        new ExifFormatter(ExifCategory.Composite, 'Light Value', 'lightValue', this.fourDecimals),
        new ExifFormatter(ExifCategory.Composite, 'Scale Factor 35 EFL', 'scaleFactor35Efl', null),
        new ExifFormatter(ExifCategory.Composite, 'Shutter Speed', 'shutterSpeed', null),
        new ExifFormatter(ExifCategory.Processing, 'Raw Conversion Mode', 'rawConversionMode', null),
        new ExifFormatter(ExifCategory.Processing, 'Sigmoidal Contrast Adjustment', 'sigmoidalContrastAdjustment', this.fourDecimals),
        new ExifFormatter(ExifCategory.Processing, 'Saturation Adjustment', 'saturationAdjustment', this.fourDecimals),
        new ExifFormatter(ExifCategory.Processing, 'Compression Quality', 'compressionQuality', null),
    ];

    exifInfo: PhotoExifInfo;

    @Input() set photo(value: Photo) {
        this._photo = value;

        if (this._photo.exif) {
            this.exifInfo = this._photo.exif;
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
                let exifInfo = new PhotoExifInfo()

                for(let fmt of this._map) {
                    let item = new ExifDetail(fmt.displayName, this.formatExif((<any>exif)[fmt.fieldName], fmt.formatFunction));

                    switch(fmt.category) {
                        case ExifCategory.Exif:
                            exifInfo.exifList.push(item);
                            break;
                        case ExifCategory.Maker:
                            exifInfo.makerList.push(item);
                            break;
                        case ExifCategory.Composite:
                            exifInfo.compositeList.push(item);
                            break;
                        case ExifCategory.Processing:
                            exifInfo.processingList.push(item);
                            break;
                    }
                }

                this.exifInfo = exifInfo;
                this.photo.exif = exifInfo;
            });
    }
}
