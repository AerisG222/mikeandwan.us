import { ThumbnailInfo } from '../../../../ng_maw/src/app/thumbnail-list';

import { Photo } from './';
 
export class PhotoThumbnailInfo extends ThumbnailInfo {
	constructor(imageUrl : string, 
	            imageHeight : number, 
				imageWidth : number,
				public photo : Photo) { 
		super(imageUrl, imageHeight, imageWidth, null, null);
	}
}
