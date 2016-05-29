import { IVideoInfo } from './index';

export interface ICategory {
	id : number;
	name : string;
	year : number;
	teaserThumbnail : IVideoInfo;
}
