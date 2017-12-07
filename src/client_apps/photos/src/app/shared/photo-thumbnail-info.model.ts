import { Photo } from './photo.model';

export class PhotoThumbnailInfo {
    constructor(imageUrl: string,
                imageHeight: number,
                imageWidth: number,
                public photo: Photo) {

    }
}
