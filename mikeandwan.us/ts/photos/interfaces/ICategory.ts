import { IPhotoInfo } from './IPhotoInfo';

export interface ICategory {
    id : number;
    name : string;
    year :number;
    hasGpsData : boolean;
    teaserPhotoInfo : IPhotoInfo;
}
