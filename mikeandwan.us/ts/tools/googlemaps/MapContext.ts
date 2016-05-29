import { BoundingBox } from './BoundingBox';

export class MapContext {
	constructor(public zoom : number, public center : google.maps.LatLng, public boundingBox : BoundingBox) { }
}
