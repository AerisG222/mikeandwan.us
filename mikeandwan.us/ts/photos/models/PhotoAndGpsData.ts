import { IPhoto } from '../interfaces/IPhoto';
import { IGpsData } from '../interfaces/IGpsData';

export class PhotoAndGpsData {
	constructor(public photo : IPhoto, public gpsData : IGpsData) {
		
	}
}
