import { ICategory, IVideoInfo } from './index';

export interface IVideo {
	id : number;
	category : ICategory;
	scaledVideo : IVideoInfo;
	fullsizeVideo : IVideoInfo;
	thumbnailVideo : IVideoInfo;
}
