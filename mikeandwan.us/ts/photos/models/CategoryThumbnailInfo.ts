import { ThumbnailInfo } from '../../ng_maw/thumbnailList/ThumbnailInfo';
import { CategoryBreadcrumb } from '../models/CategoryBreadcrumb';
 
export class CategoryThumbnailInfo extends ThumbnailInfo {
	constructor(imageUrl : string, 
	            imageHeight : number, 
				imageWidth : number,
				public category : CategoryBreadcrumb,
				title : string,
				icon : string) { 
		super(imageUrl, imageHeight, imageWidth, title, icon);
	}
}
