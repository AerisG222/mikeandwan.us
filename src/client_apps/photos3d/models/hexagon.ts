export class Hexagon {
    private static readonly AREA_MULTIPLIER = 3 * Math.sqrt(3) / 2;
    private halfEdgeLength: number;
    private totalHeight: number;
    private halfHeight: number;

    constructor(private edgeLength: number) {
        // a^2 + (edgelength/2)^2 = edgelength^2
        this.halfEdgeLength = this.edgeLength * 0.5;
        this.halfHeight = Math.sqrt(Math.pow(this.edgeLength, 2) - Math.pow(this.halfEdgeLength, 2));
        this.totalHeight = this.halfHeight * 2;
    }

    get centerToVertexLength() {
        return this.edgeLength;
    }

    get centerToMidEdgeLength() {
        return this.halfHeight;
    }

    area(): number {
        // cool: search google for 'area of hexagon'
        return Hexagon.AREA_MULTIPLIER * Math.pow(this.edgeLength, 2);
    }

    generatePoints(): Array<THREE.Vector2> {
        let points: Array<THREE.Vector2> = [];

        points.push(new THREE.Vector2(-this.halfEdgeLength, this.halfHeight));
        points.push(new THREE.Vector2(this.halfEdgeLength, this.halfHeight));
        points.push(new THREE.Vector2(this.edgeLength, 0));
        points.push(new THREE.Vector2(this.halfEdgeLength, -this.halfHeight));
        points.push(new THREE.Vector2(-this.halfEdgeLength, -this.halfHeight));
        points.push(new THREE.Vector2(-this.edgeLength, 0));
        points.push(new THREE.Vector2(-this.halfEdgeLength, this.halfHeight));

        return points;
    }
}
