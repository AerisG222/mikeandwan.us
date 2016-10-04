import { IPhotoInfo } from './iphoto-info.model';

export interface IPhoto {
    id: number;
    categoryId: number;
    latitude: number;
    longitude: number;
    xsInfo: IPhotoInfo;
    smInfo: IPhotoInfo;
    mdInfo: IPhotoInfo;
    lgInfo: IPhotoInfo;
    prtInfo: IPhotoInfo;
}
