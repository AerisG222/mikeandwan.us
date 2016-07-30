import { ThumbnailInfo } from '../../ng_maw/thumbnail-list/thumbnail-info.model';

import { Photo } from './photo.model';

export class PhotoThumbnailInfo extends ThumbnailInfo {
    constructor(imageUrl: string,
                imageHeight: number,
                imageWidth: number,
                public photo: Photo) {
        super(imageUrl, imageHeight, imageWidth, null, null);
    }
}
