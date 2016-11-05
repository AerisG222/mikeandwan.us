import { CategoryObject3D } from './category-object3d';

export interface IYear {
    year: number;
    color: string;
    categories: Array<CategoryObject3D>;
}
