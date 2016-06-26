import { IPhoto } from './iphoto.model';
import { IGpsData } from './igps-data.model';

export class PhotoAndGpsData {
    constructor(public photo: IPhoto, public gpsData: IGpsData) {

    }
}
