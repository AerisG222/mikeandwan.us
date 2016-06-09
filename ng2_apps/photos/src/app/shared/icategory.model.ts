import { IPhotoInfo } from './';

export interface ICategory {
    id: number;
    name: string;
    year: number;
    hasGpsData: boolean;
    teaserPhotoInfo: IPhotoInfo;
}
