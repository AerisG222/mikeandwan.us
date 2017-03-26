import { SvgIcon } from '../svg-icon/svg-icon.enum';

export class ThumbnailInfo {
    constructor(public imageUrl: string,
                public imageHeight: number,
                public imageWidth: number,
                public title?: string,
                public icon?: SvgIcon) {

    }
}
