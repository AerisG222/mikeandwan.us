import { ICategory } from './ICategory';
import { IVideoInfo } from './IVideoInfo';

export interface IVideo {
	id : number;
	category : ICategory;
	scaledVideo : IVideoInfo;
	fullsizeVideo : IVideoInfo;
	thumbnailVideo : IVideoInfo;
}
