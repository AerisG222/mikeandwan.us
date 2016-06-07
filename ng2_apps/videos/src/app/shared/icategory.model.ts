import { IVideoInfo } from './';

export interface ICategory {
	id : number;
	name : string;
	year : number;
	teaserThumbnail : IVideoInfo;
}
