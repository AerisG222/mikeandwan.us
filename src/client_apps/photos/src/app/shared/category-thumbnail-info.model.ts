import { SvgIcon } from '../../ng_maw/svg-icon/svg-icon.enum';

import { CategoryBreadcrumb } from './category-breadcrumb.model';

export class CategoryThumbnailInfo {
    constructor(imageUrl: string,
                imageHeight: number,
                imageWidth: number,
                public category: CategoryBreadcrumb,
                title: string,
                icon: SvgIcon) {

    }
}
