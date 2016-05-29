import { ThumbnailInfo } from '../../../../ng_maw/src/app/thumbnail-list/thumbnail-info';
import { IVideo } from './index';
 
export class VideoThumbnailInfo extends ThumbnailInfo {
	constructor(imageUrl : string, 
	            imageHeight : number, 
				imageWidth : number,
				public video : IVideo,
				title? : string,
				icon? : string) { 
		super(imageUrl, imageHeight, imageWidth, title, icon);
	}
}
