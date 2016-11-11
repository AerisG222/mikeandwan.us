export class Hexagon {
    private halfEdgeLength: number;
    private totalLength: number;
    private totalHeight: number;
    private halfHeight: number;

    constructor(private edgeLength: number) {
        // a^2 + (edgelength/2)^2 = edgelength^2
        this.halfEdgeLength = this.edgeLength * 0.5;
        this.halfHeight = Math.sqrt(Math.pow(this.edgeLength, 2) - Math.pow(this.halfEdgeLength, 2));
        this.totalHeight = this.halfHeight * 2;
    }

    generatePoints(): Array<THREE.Vector2> {
        let points: Array<THREE.Vector2> = [];

        points.push(new THREE.Vector2(this.halfEdgeLength, 0));
        points.push(new THREE.Vector2(-this.halfEdgeLength, 0));
        points.push(new THREE.Vector2(-this.edgeLength, this.halfHeight));
        points.push(new THREE.Vector2(-this.halfEdgeLength, this.totalHeight));
        points.push(new THREE.Vector2(this.halfEdgeLength, this.totalHeight));
        points.push(new THREE.Vector2(this.edgeLength, this.halfHeight));
        points.push(new THREE.Vector2(this.halfEdgeLength, 0));

        return points;
    }
}
