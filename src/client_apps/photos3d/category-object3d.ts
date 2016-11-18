import { ICategory } from './icategory';
import { Hexagon } from './hexagon';
import { StateService } from './state-service';

export class CategoryObject3D extends THREE.Object3D {
    private static BACKGROUND_DEPTH = 0.1;
    private static IMAGE_DEPTH = 0.3;
    private static BORDER_WIDTH = 2;
    private static loader = new THREE.TextureLoader();
    
    private isMouseOver = false;
    private isMouseOverTextureSet = false;
    private counter = Math.random() * 2 * Math.PI;
    private backgroundMesh: THREE.Mesh = null;
    private imageMesh: THREE.Mesh = null;

    constructor(private stateService: StateService,
                private category: ICategory,
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

        this.stateService.MouseoverObservable.subscribe(x => this.onMouseOver(x));
    }

    render(delta: number) {
        if(this.isMouseOver) {
            if(!this.isMouseOverTextureSet) {
                this.backgroundMesh.material = new THREE.MeshLambertMaterial({ color: 255, side: THREE.DoubleSide });
                this.isMouseOverTextureSet = true;
            }

            if(this.position.z < 12) {
                this.position.z += 2;

                if(this.position.z > 12) {
                    this.position.z = 12;
                }
            }
        }
        else {
            if(this.isMouseOverTextureSet) {
                this.backgroundMesh.material = new THREE.MeshLambertMaterial({ color: this.color, side: THREE.DoubleSide });
                this.isMouseOverTextureSet = false;
            }

            if(this.position.z > 0) {
                this.position.z -= 0.2;

                if(this.position.z < 0) {
                    this.position.z = 0;
                }
            }
        }
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

    private onMouseOver(intersections: Array<THREE.Intersection>) {
        if(intersections.length == 0) {
            this.isMouseOver = false;
            this.position.z = 0;
        }
        else {
            let isMouseOver = false;

            for(var i = 0; i < intersections.length; i++) {
                if(intersections[i].object.parent instanceof CategoryObject3D) {
                    if(this.uuid == intersections[i].object.parent.uuid) {
                        isMouseOver = true;

                        this.stateService.updateTemporalNav(this.category.name);

                        break;
                    }
                }
            }

            this.isMouseOver = isMouseOver;
        }
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
