import { IVideoInfo } from './IVideoInfo';

export interface ICategory {
	id : number;
	name : string;
	year : number;
	teaserThumbnail : IVideoInfo;
}
