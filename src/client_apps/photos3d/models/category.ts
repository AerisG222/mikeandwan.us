import { ArgumentNullError } from './argument-null-error';
import { ICategory } from './icategory';
import { CategoryVisual } from '../visuals/category-visual';

export class Category {
    constructor(private _category: ICategory,
                private _visual: CategoryVisual) {
        if (_category == null) {
            throw new ArgumentNullError('_category');
        }

        if (_visual == null) {
            throw new ArgumentNullError('_visual');
        }
    }

    get category(): ICategory {
        return this._category;
    }

    get visual(): CategoryVisual {
        return this._visual;
    }
}
