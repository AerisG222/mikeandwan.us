import {ExifCategory} from './exif-category.model';

export class ExifFormatter {
    constructor(public category: ExifCategory,
                public displayName: string,
                public fieldName: string,
                public formatFunction: Function) {

    }
}
