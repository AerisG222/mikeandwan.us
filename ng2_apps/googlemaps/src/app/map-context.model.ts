import { BoundingBox } from './bounding-box.model';

export class MapContext {
    constructor(public zoom: number, public center: google.maps.LatLng, public boundingBox: BoundingBox) {

    }
}
