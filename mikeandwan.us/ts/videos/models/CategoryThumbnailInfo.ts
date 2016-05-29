import { ThumbnailInfo } from '../../ng_maw/thumbnailList/ThumbnailInfo';
import { ICategory } from '../interfaces/ICategory';
 
export class CategoryThumbnailInfo extends ThumbnailInfo {
	constructor(imageUrl : string, 
	            imageHeight : number, 
				imageWidth : number,
				public category : ICategory,
				title? : string,
				icon? : string) { 
		super(imageUrl, imageHeight, imageWidth, title, icon);
	}
}
