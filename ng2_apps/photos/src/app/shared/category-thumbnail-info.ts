import { ThumbnailInfo } from '../../../../ng_maw/src/app/thumbnail-list';
import { CategoryBreadcrumb } from './';
 
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
