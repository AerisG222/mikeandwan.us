import { Instruction } from '@angular/router';

import { Breadcrumb } from '../../../../ng_maw/src/app/shared/breadcrumb.model';

import { ICategory } from './icategory.model';

export class CategoryBreadcrumb extends Breadcrumb {
    constructor(title: string,
        routerInstruction: Instruction,
        public category: ICategory) {
        super(title, routerInstruction);
    }
}
