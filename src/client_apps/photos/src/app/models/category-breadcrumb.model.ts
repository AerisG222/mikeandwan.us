import { Breadcrumb } from './breadcrumb.model';
import { ICategory } from './icategory.model';

export class CategoryBreadcrumb extends Breadcrumb {
    constructor(title: string,
                linkParamArray: Array<any>,
                public category: ICategory) {
        super(title, linkParamArray);
    }
}
