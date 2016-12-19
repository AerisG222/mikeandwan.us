export class Hexagon {
    private static readonly AREA_MULTIPLIER = 3 * Math.sqrt(3) / 2;
    private _halfEdgeLength: number;
    private _totalHeight: number;
    private _halfHeight: number;

    constructor(private _edgeLength: number) {
        // a^2 + (edgelength/2)^2 = edgelength^2
        this._halfEdgeLength = this._edgeLength * 0.5;
        this._halfHeight = Math.sqrt(Math.pow(this._edgeLength, 2) - Math.pow(this._halfEdgeLength, 2));
        this._totalHeight = this._halfHeight * 2;
    }

    get centerToVertexLength() {
        return this._edgeLength;
    }

    get centerToMidEdgeLength() {
        return this._halfHeight;
    }

    area(): number {
        // cool: search google for 'area of hexagon'
        return Hexagon.AREA_MULTIPLIER * Math.pow(this._edgeLength, 2);
    }

    generatePoints(): Array<THREE.Vector2> {
        let points: Array<THREE.Vector2> = [];

        points.push(new THREE.Vector2(-this._halfEdgeLength, this._halfHeight));
        points.push(new THREE.Vector2(-this._edgeLength, 0));
        points.push(new THREE.Vector2(-this._halfEdgeLength, -this._halfHeight));
        points.push(new THREE.Vector2(this._halfEdgeLength, -this._halfHeight));
        points.push(new THREE.Vector2(this._edgeLength, 0));
        points.push(new THREE.Vector2(this._halfEdgeLength, this._halfHeight));
        points.push(new THREE.Vector2(-this._halfEdgeLength, this._halfHeight));

        return points;
    }
}
