import { ExifDetail } from '../exif-view/exif-detail.model';

export class PhotoExifInfo {
    exifList: Array<ExifDetail> = [];
    makerList: Array<ExifDetail> = [];
    compositeList: Array<ExifDetail> = [];
    processingList: Array<ExifDetail> = [];
}
