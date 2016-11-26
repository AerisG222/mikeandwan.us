import { ICategory } from '../models/icategory';
import { Hexagon } from '../models/hexagon';
import { StateService } from '../services/state-service';

export class CategoryVisual extends THREE.Object3D {
    private static BACKGROUND_DEPTH = 0.1;
    private static IMAGE_DEPTH = 0.3;
    private static BORDER_WIDTH = 2;
    private static loader = new THREE.TextureLoader();
    
    private isMouseOver = false;
    private isMouseOverTextureSet = false;
    private counter = Math.random() * 2 * Math.PI;
    private backgroundMesh: THREE.Mesh = null;
    private imageMesh: THREE.Mesh = null;
    private _bringIntoViewTween: TWEEN.Tween;
    private _removeFromViewTween: TWEEN.Tween;

    constructor(private stateService: StateService,
                private category: ICategory,
                private hexagon: Hexagon,
                private onscreenPosition: THREE.Vector3,
                private offscreenPosition: THREE.Vector3,
                private color: number) {
        super();

        this.position.set(offscreenPosition.x, offscreenPosition.y, offscreenPosition.z);
    }

    init() {
        CategoryVisual.loader.load(this.category.teaserImage.path, texture => {
            this.createObject(texture);
        });

        this.createBackground();
        this.add(this.backgroundMesh);

        this.stateService.mouseoverObservable.subscribe(x => this.onMouseOver(x));
    }

    bringIntoView(): void {
        if(this._removeFromViewTween != null) {
            this._removeFromViewTween.stop();
            this._removeFromViewTween = null;
        }

        this._bringIntoViewTween = new TWEEN.Tween(this.position)
            .to(this.onscreenPosition, 1200)
            .easing(TWEEN.Easing.Back.Out)
            .start();
    }

    removeFromView(): void {
        if(this._bringIntoViewTween != null) {
            this._bringIntoViewTween.stop();
            this._bringIntoViewTween = null;
        }

        this._removeFromViewTween = new TWEEN.Tween(this.position)
            .to(this.offscreenPosition, 1200)
            .easing(TWEEN.Easing.Sinusoidal.Out)
            .start();
    }

    render(delta: number) {
        /*
        if(this.isMouseOver) {
            if(!this.isMouseOverTextureSet) {
                this.backgroundMesh.material = new THREE.MeshLambertMaterial({ color: 255, side: THREE.DoubleSide });
                this.isMouseOverTextureSet = true;
            }

            if(this.position.z < this.zStart + 12) {
                this.position.z += 2;

                if(this.position.z > this.zStart + 12) {
                    this.position.z = this.zStart + 12;
                }
            }
        }
        else {
            if(this.isMouseOverTextureSet) {
                this.backgroundMesh.material = new THREE.MeshLambertMaterial({ color: this.color, side: THREE.DoubleSide });
                this.isMouseOverTextureSet = false;
            }

            if(this.position.z > this.zStart) {
                this.position.z -= 0.2;

                if(this.position.z < this.zStart) {
                    this.position.z = this.zStart;
                }
            }
        }
        */
    }

    private createObject(texture: THREE.Texture) {
        this.createImage(texture);
        this.add(this.imageMesh);
    }

    private createBackground() {
        let len = this.hexagon.centerToVertexLength + CategoryVisual.BORDER_WIDTH;
        let geometry = this.createExtrudeGeometry(len, CategoryVisual.BACKGROUND_DEPTH);
        let material = new THREE.MeshLambertMaterial({ color: this.color, side: THREE.DoubleSide });
        
        this.backgroundMesh = new THREE.Mesh(geometry, material);
    }

    private createImage(texture: THREE.Texture) {
        let geometry = this.createExtrudeGeometry(this.hexagon.centerToVertexLength, CategoryVisual.IMAGE_DEPTH);

        this.mapUvs(geometry);

        let material = new THREE.MeshBasicMaterial({ map: texture, side: THREE.DoubleSide });

        this.imageMesh = new THREE.Mesh(geometry, material);

        this.imageMesh.position.y = CategoryVisual.BORDER_WIDTH / 2;
        this.imageMesh.position.z = (CategoryVisual.IMAGE_DEPTH - CategoryVisual.BACKGROUND_DEPTH) / 2;
    }

    private onMouseOver(intersections: Array<THREE.Intersection>) {
        intersections = intersections.filter(x => x.object.parent instanceof CategoryVisual);

        if(intersections.length == 0) {
            this.isMouseOver = false;
        }
        else {
            let isMouseOver = false;

            for(var i = 0; i < intersections.length; i++) {
                if(intersections[i].object.parent instanceof CategoryVisual) {
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
