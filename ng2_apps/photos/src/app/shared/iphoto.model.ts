import { IPhotoInfo } from './iphoto-info.model';

export interface IPhoto {
    id: number;
    categoryId: number;
    latitude: number;
    longitude: number;
    thumbnailInfo: IPhotoInfo;
    fullsizeInfo: IPhotoInfo;
    fullerInfo: IPhotoInfo;
    originalInfo: IPhotoInfo;
}
