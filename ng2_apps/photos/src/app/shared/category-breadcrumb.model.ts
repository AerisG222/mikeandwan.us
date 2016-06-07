import { Instruction } from '@angular/router';

import { Breadcrumb } from '../../../../ng_maw/src/app/shared';

import { ICategory } from './';

export class CategoryBreadcrumb extends Breadcrumb {
	constructor(title : string,
				routerInstruction : Instruction,
				public category : ICategory) {
		super(title, routerInstruction);
	}
}
