import { ThumbnailInfo } from '../../ng_maw/thumbnailList/ThumbnailInfo';
import { Photo } from '../models/Photo';
 
export class PhotoThumbnailInfo extends ThumbnailInfo {
	constructor(imageUrl : string, 
	            imageHeight : number, 
				imageWidth : number,
				public photo : Photo) { 
		super(imageUrl, imageHeight, imageWidth, null, null);
	}
}
