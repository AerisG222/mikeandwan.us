import { IPhoto, ICategory, ExifDetail, FilterSettings } from './';

export class Photo {
	filters : FilterSettings = new FilterSettings();
	rotationClassIndex : number = 0;
	
	constructor(public photo : IPhoto, public category : ICategory, public exif? : Array<Array<ExifDetail>>) {
		
	}
	
	get hasGps() : boolean {
		return this.photo.latitude != null;
	}
}
