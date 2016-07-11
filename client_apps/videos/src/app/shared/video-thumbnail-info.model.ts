import { ThumbnailInfo } from '../../ng_maw/thumbnail-list/';

import { IVideo } from './ivideo.model';

export class VideoThumbnailInfo extends ThumbnailInfo {
    constructor(imageUrl: string,
        imageHeight: number,
        imageWidth: number,
        public video: IVideo,
        title?: string,
        icon?: string) {
        super(imageUrl, imageHeight, imageWidth, title, icon);
    }
}
