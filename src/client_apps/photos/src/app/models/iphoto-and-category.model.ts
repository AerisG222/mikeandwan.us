import { ICategory } from './icategory.model';
import { IPhoto } from './iphoto.model';

export interface IPhotoAndCategory {
    photo: IPhoto;
    category: ICategory;
}
