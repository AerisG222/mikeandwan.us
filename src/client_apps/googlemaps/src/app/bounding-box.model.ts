export class BoundingBox {
    upperLeft: google.maps.LatLng;
    upperRight: google.maps.LatLng;
    lowerLeft: google.maps.LatLng;
    lowerRight: google.maps.LatLng;

    constructor(sourceMap: google.maps.Map) {
        const bounds = sourceMap.getBounds();

        if (bounds) {
            const minY = bounds.getSouthWest().lat();
            const minX = bounds.getSouthWest().lng();
            const maxY = bounds.getNorthEast().lat();
            const maxX = bounds.getNorthEast().lng();

            this.upperLeft = new google.maps.LatLng(maxY, minX);
            this.upperRight = new google.maps.LatLng(maxY, maxX);
            this.lowerRight = new google.maps.LatLng(minY, maxX);
            this.lowerLeft = new google.maps.LatLng(minY, minX);
        }
    }

    get boundsArray(): Array<google.maps.LatLng> {
        const points: Array<google.maps.LatLng> = new Array(5);

        points[0] = this.upperLeft;
        points[1] = this.upperRight;
        points[2] = this.lowerRight;
        points[3] = this.lowerLeft;
        points[4] = this.upperLeft;

        return points;
    }
}
