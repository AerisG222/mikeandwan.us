import { ThumbnailInfo } from '../../../../ng_maw/src/app/thumbnail-list/thumbnail-info.model';

import { ICategory } from './icategory.model';

export class CategoryThumbnailInfo extends ThumbnailInfo {
    constructor(imageUrl: string,
                imageHeight: number,
                imageWidth: number,
                public category: ICategory,
                title?: string,
                icon?: string) {
        super(imageUrl, imageHeight, imageWidth, title, icon);
    }
}
