import { Breadcrumb } from '../../../../ng_maw/src/app/shared/breadcrumb.model';

import { ICategory } from './icategory.model';

export class CategoryBreadcrumb extends Breadcrumb {
    constructor(title: string,
                linkParamArray: Array<any>,
                public category: ICategory) {
        super(title, linkParamArray);
    }
}
