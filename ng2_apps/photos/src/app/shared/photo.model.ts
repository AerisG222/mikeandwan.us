import { IPhoto } from './iphoto.model';
import { ICategory } from './icategory.model';
import { ExifDetail } from './exif-detail.model';
import { FilterSettings } from './filter-settings.model';

export class Photo {
    filters: FilterSettings = new FilterSettings();
    rotationClassIndex: number = 0;

    constructor(public photo: IPhoto, public category: ICategory, public exif?: Array<Array<ExifDetail>>) {

    }

    get hasGps(): boolean {
        return this.photo.latitude != null;
    }
}
