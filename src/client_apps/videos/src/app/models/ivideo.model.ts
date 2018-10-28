import { ICategory } from './icategory.model';
import { IVideoInfo } from './ivideo-info.model';

export interface IVideo {
    id: number;
    category: ICategory;
    scaledVideo: IVideoInfo;
    fullsizeVideo: IVideoInfo;
    thumbnailVideo: IVideoInfo;
}
