import { ICategory, IVideoInfo } from './';

export interface IVideo {
    id: number;
    category: ICategory;
    scaledVideo: IVideoInfo;
    fullsizeVideo: IVideoInfo;
    thumbnailVideo: IVideoInfo;
}
