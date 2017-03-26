import { SvgIcon } from '../../ng_maw/svg-icon/svg-icon.enum';
import { ThumbnailInfo } from '../../ng_maw/thumbnail-list/thumbnail-info.model';

import { CategoryBreadcrumb } from './category-breadcrumb.model';

export class CategoryThumbnailInfo extends ThumbnailInfo {
    constructor(imageUrl: string,
                imageHeight: number,
                imageWidth: number,
                public category: CategoryBreadcrumb,
                title: string,
                icon: SvgIcon) {
        super(imageUrl, imageHeight, imageWidth, title, icon);
    }
}
