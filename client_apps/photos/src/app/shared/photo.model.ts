import { IPhoto } from './iphoto.model';
import { ICategory } from './icategory.model';
import { FilterSettings } from './filter-settings.model';
import { PhotoExifInfo } from './photo-exif-info.model';

export class Photo {
    filters: FilterSettings = new FilterSettings();
    rotationClassIndex: number = 0;

    constructor(public photo: IPhoto, public category: ICategory, public exif?: PhotoExifInfo) {

    }

    get hasGps(): boolean {
        return this.photo.latitude != null;
    }
}
