import { Breadcrumb } from '../../ng_maw/services/Breadcrumb';
import { Instruction } from '@angular/router-deprecated';
import { ICategory } from '../interfaces/ICategory';

export class CategoryBreadcrumb extends Breadcrumb {
	constructor(title : string,
				routerInstruction : Instruction,
				public category : ICategory) {
		super(title, routerInstruction);
	}
}
