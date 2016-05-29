import { IPhoto } from '../interfaces/IPhoto';
import { ICategory } from '../interfaces/ICategory';
import { ExifDetail } from '../models/ExifDetail';
import { FilterSettings } from '../models/FilterSettings';

export class Photo {
	filters : FilterSettings = new FilterSettings();
	rotationClassIndex : number = 0;
	
	constructor(public photo : IPhoto, public category : ICategory, public exif? : Array<Array<ExifDetail>>) {
		
	}
	
	get hasGps() : boolean {
		return this.photo.latitude != null;
	}
}
