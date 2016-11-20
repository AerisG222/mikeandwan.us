import { IImage } from './iimage';

export interface ICategory {
    id: number;
    name: string;
    year: number;
    teaserImage: IImage;
}
