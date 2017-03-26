import { SvgIcon } from '../../ng_maw/svg-icon/svg-icon.enum';
import { ThumbnailInfo } from '../../ng_maw/thumbnail-list/';

import { IVideo } from './ivideo.model';

export class VideoThumbnailInfo extends ThumbnailInfo {
    constructor(imageUrl: string,
                imageHeight: number,
                imageWidth: number,
                public video: IVideo,
                title?: string,
                icon?: SvgIcon) {
        super(imageUrl, imageHeight, imageWidth, title, icon);
    }
}
