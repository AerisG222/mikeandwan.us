import { ICategory } from './icategory';
import { CategoryObject3D } from './category-object3d';

export interface IYear {
    year: number;
    index: number;
    color: number;
    categories: Array<ICategory>;
    categoryObjects: Array<CategoryObject3D>;
}
