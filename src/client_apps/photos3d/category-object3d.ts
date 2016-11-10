import { ICategory } from './icategory';

export class CategoryObject3D extends THREE.Object3D {
    private static EDGE_LENGTH = 24;
    private static BORDER_WIDTH = 2;
    private static loader = new THREE.TextureLoader();

    private backgroundMesh: THREE.Mesh = null;
    private imageMesh: THREE.Mesh = null;

    constructor(private category: ICategory,
                private endPosition: THREE.Vector3,
                private color: number) {
        super();

        this.position.x = this.endPosition.x;
        this.position.y = this.endPosition.y;
        this.position.z = this.endPosition.z;
    }

    init() {
        CategoryObject3D.loader.load(this.category.teaserImage.path, texture => {
            this.createObject(texture);
        });

        this.createBackground();
        this.add(this.backgroundMesh);
    }

    render(delta: number) {
        if(this.position.equals(this.endPosition)) {
            return;
        }
    }

    private createObject(texture: THREE.Texture) {
        this.createImage(texture);
        this.add(this.imageMesh);
    }

    private createBackground() {
        let len = CategoryObject3D.EDGE_LENGTH;
        let geometry = this.createExtrudeGeometry(CategoryObject3D.EDGE_LENGTH, 2);
        let material = new THREE.MeshLambertMaterial({ color: this.color, side: THREE.DoubleSide });
        
        this.backgroundMesh = new THREE.Mesh(geometry, material);
    }

    private createImage(texture: THREE.Texture) {
        let len = CategoryObject3D.EDGE_LENGTH - CategoryObject3D.BORDER_WIDTH;
        let geometry = this.createExtrudeGeometry(len, 6);

        this.mapUvs(geometry);

        let material = new THREE.MeshBasicMaterial({ map: texture, side: THREE.DoubleSide });

        this.imageMesh = new THREE.Mesh(geometry, material);

        this.imageMesh.position.y = CategoryObject3D.BORDER_WIDTH / 2;
        this.imageMesh.position.z = -3;
    }

    // https://github.com/mrdoob/three.js/issues/2065
    private mapUvs(geometry: THREE.Geometry) {
        geometry.computeBoundingBox();
        let minX = geometry.boundingBox.min.x;
        let minY = geometry.boundingBox.min.y;
        let maxX = geometry.boundingBox.max.x;
        let maxY = geometry.boundingBox.max.y;
        let deltaX = maxX - minX;
        let deltaY = maxY - minY;

        let faceCount = geometry.faces.length;
        let uvA = new THREE.Vector2(0, 0);
        let uvB = new THREE.Vector2(0, 0);
        let uvC = new THREE.Vector2(0, 0);

        geometry.faceVertexUvs[0] = [];

        for(let faceIdx = 0; faceIdx < faceCount; ++faceIdx)
        {
            let face = geometry.faces[faceIdx];
            let vtx = geometry.vertices[face.a];
            uvA.set((vtx.x - minX) / deltaX, (vtx.y - minY) / deltaY);

            vtx = geometry.vertices[ face.b ];
            uvB.set(( vtx.x - minX) / deltaX, (vtx.y - minY) / deltaY);

            vtx = geometry.vertices[face.c];
            uvC.set((vtx.x - minX) / deltaX, (vtx.y - minY) / deltaY);

            geometry.faceVertexUvs[0].push([uvA.clone(), uvB.clone(), uvC.clone()]);
        }
    }

    private createExtrudeGeometry(edgeLength: number, depth: number): THREE.ExtrudeGeometry {
        let points: Array<THREE.Vector2> = [];

        // a^2 + (edgelength/2)^2 = edgelength^2
        let halfLength = edgeLength / 2;
        let halfHeight = Math.sqrt(Math.pow(edgeLength, 2) - Math.pow(halfLength, 2));
        let totalHeight = halfHeight * 2;

        points.push(new THREE.Vector2(halfLength, 0));
        points.push(new THREE.Vector2(-halfLength, 0));
        points.push(new THREE.Vector2(-edgeLength, halfHeight));
        points.push(new THREE.Vector2(-halfLength, totalHeight));
        points.push(new THREE.Vector2(halfLength, totalHeight));
        points.push(new THREE.Vector2(edgeLength, halfHeight));
        points.push(new THREE.Vector2(halfLength, 0));

        let shape = new THREE.Shape(points);

        let settings = { 
            amount: depth,
            bevelEnabled: false
        };

        return new THREE.ExtrudeGeometry(shape, settings);
    }
}
