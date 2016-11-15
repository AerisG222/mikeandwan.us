import { ICategory } from './icategory';
import { Hexagon } from './hexagon';

export class CategoryObject3D extends THREE.Object3D {
    private static BACKGROUND_DEPTH = 0.1;
    private static IMAGE_DEPTH = 0.3;
    private static BORDER_WIDTH = 2;
    private static loader = new THREE.TextureLoader();
    
    private counter = Math.random() * 2 * Math.PI;
    private backgroundMesh: THREE.Mesh = null;
    private imageMesh: THREE.Mesh = null;

    constructor(private category: ICategory,
                private hexagon: Hexagon,
                private endPosition: THREE.Vector2,
                private color: number) {
        super();

        this.position.x = this.endPosition.x;
        this.position.y = this.endPosition.y;
        this.position.z = 0;
    }

    init() {
        CategoryObject3D.loader.load(this.category.teaserImage.path, texture => {
            this.createObject(texture);
        });

        this.createBackground();
        this.add(this.backgroundMesh);
    }

    render(delta: number) {
        this.counter += (2 * delta);
        this.position.z += Math.sin(this.counter);
    }

    private createObject(texture: THREE.Texture) {
        this.createImage(texture);
        this.add(this.imageMesh);
    }

    private createBackground() {
        let len = this.hexagon.centerToVertexLength + CategoryObject3D.BORDER_WIDTH;
        let geometry = this.createExtrudeGeometry(len, CategoryObject3D.BACKGROUND_DEPTH);
        let material = new THREE.MeshLambertMaterial({ color: this.color, side: THREE.DoubleSide });
        
        this.backgroundMesh = new THREE.Mesh(geometry, material);
    }

    private createImage(texture: THREE.Texture) {
        let geometry = this.createExtrudeGeometry(this.hexagon.centerToVertexLength, CategoryObject3D.IMAGE_DEPTH);

        this.mapUvs(geometry);

        let material = new THREE.MeshBasicMaterial({ map: texture, side: THREE.DoubleSide });

        this.imageMesh = new THREE.Mesh(geometry, material);

        this.imageMesh.position.y = CategoryObject3D.BORDER_WIDTH / 2;
        this.imageMesh.position.z = (CategoryObject3D.IMAGE_DEPTH - CategoryObject3D.BACKGROUND_DEPTH) / 2;
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
        let hex = new Hexagon(edgeLength);
        let shape = new THREE.Shape(hex.generatePoints());
        let settings = { 
            amount: depth,
            bevelEnabled: false
        };

        return new THREE.ExtrudeGeometry(shape, settings);
    }
}
