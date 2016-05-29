import { ThumbnailInfo } from '../../ng_maw/thumbnailList/ThumbnailInfo';
import { IVideo } from '../interfaces/IVideo';
 
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
