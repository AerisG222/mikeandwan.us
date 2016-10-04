import { IPhotoInfo } from './iphoto-info.model';

export interface ICategory {
    id: number;
    name: string;
    year: number;
    hasGpsData: boolean;
    teaserPhotoInfo: IPhotoInfo;
}
