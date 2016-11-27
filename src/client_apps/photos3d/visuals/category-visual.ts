import { ICategory } from '../models/icategory';
import { IVisual } from './ivisual';
import { Hexagon } from '../models/hexagon';
import { StateService } from '../services/state-service';

export class CategoryVisual extends THREE.Object3D implements IVisual {
    private static readonly BACKGROUND_DEPTH = 0.1;
    private static readonly IMAGE_DEPTH = 0.3;
    private static readonly BORDER_WIDTH = 2;

    private static loader = new THREE.TextureLoader();

    private _ignoreMouseEvents = true;
    private _isMouseOver = false;
    private _backgroundMesh: THREE.Mesh = null;
    private _imageMesh: THREE.Mesh = null;
    private _hoverPosition: THREE.Vector3;
    private _bringIntoViewTween: TWEEN.Tween;
    private _removeFromViewTween: TWEEN.Tween;
    private _mouseOverTween: TWEEN.Tween;
    private _mouseOutTween: TWEEN.Tween;

    constructor(private stateService: StateService,
                private category: ICategory,
                private hexagon: Hexagon,
                private onscreenPosition: THREE.Vector3,
                private offscreenPosition: THREE.Vector3,
                private color: number) {
        super();

        this.position.set(offscreenPosition.x, offscreenPosition.y, offscreenPosition.z);
        this._hoverPosition = new THREE.Vector3(onscreenPosition.x, onscreenPosition.y, onscreenPosition.z + 12);
    }

    init() {
        CategoryVisual.loader.load(this.category.teaserImage.path, texture => {
            this.createObject(texture);
        });

        this.createBackground();
        this.add(this._backgroundMesh);

        this.stateService.mouseoverObservable.subscribe(x => this.onMouseEvent(x));
        this.stateService.mouseclickObservable.subscribe(x => this.onMouseClick(x));
    }

    bringIntoView(): void {
        if (this._removeFromViewTween != null) {
            this._removeFromViewTween.stop();
            this._removeFromViewTween = null;
        }

        this._bringIntoViewTween = new TWEEN.Tween(this.position)
            .to(this.onscreenPosition, 1200)
            .easing(TWEEN.Easing.Back.Out)
            .start()
            .onComplete(x => { this._ignoreMouseEvents = false; });
    }

    removeFromView(): void {
        this._ignoreMouseEvents = true;

        if (this._bringIntoViewTween != null) {
            this._bringIntoViewTween.stop();
            this._bringIntoViewTween = null;
        }

        this._removeFromViewTween = new TWEEN.Tween(this.position)
            .to(this.offscreenPosition, 1200)
            .easing(TWEEN.Easing.Sinusoidal.Out)
            .start();
    }

    render(delta: number) {
        // all rendering done by tweens now
    }

    private createObject(texture: THREE.Texture) {
        this.createImage(texture);
        this.add(this._imageMesh);
    }

    private createBackground() {
        let len = this.hexagon.centerToVertexLength + CategoryVisual.BORDER_WIDTH;
        let geometry = this.createExtrudeGeometry(len, CategoryVisual.BACKGROUND_DEPTH);
        let material = new THREE.MeshLambertMaterial({ color: this.color, side: THREE.DoubleSide });

        this._backgroundMesh = new THREE.Mesh(geometry, material);
    }

    private createImage(texture: THREE.Texture) {
        let geometry = this.createExtrudeGeometry(this.hexagon.centerToVertexLength, CategoryVisual.IMAGE_DEPTH);

        this.mapUvs(geometry);

        let material = new THREE.MeshBasicMaterial({ map: texture, side: THREE.DoubleSide });

        this._imageMesh = new THREE.Mesh(geometry, material);

        this._imageMesh.position.y = CategoryVisual.BORDER_WIDTH / 2;
        this._imageMesh.position.z = (CategoryVisual.IMAGE_DEPTH - CategoryVisual.BACKGROUND_DEPTH) / 2;
    }

    private onMouseEvent(intersections: Array<THREE.Intersection>) {
        if (this._ignoreMouseEvents) {
            return;
        }

        let isMouseOver = this.isMouseEventForThisInstance(intersections);

        if (!isMouseOver) {
            if(this._isMouseOver) {
                this.onMouseOut();
            }
        } else {
            // TODO: we currently clear the temporal status on a mouse event, expecting a hovered category to set this
            //       seems like there might be a more performant option
            this.stateService.publishTemporalNav(this.category.name);

            if (!this._isMouseOver) {
                this.onMouseOver();
            }
        }
    }

    private onMouseClick(intersections: Array<THREE.Intersection>) {
        if (this._ignoreMouseEvents) {
            return;
        }

        let isMouseOver = this.isMouseEventForThisInstance(intersections);

        if(isMouseOver) {
            this.stateService.publishTemporalNav(null);
            this.stateService.publishActiveNav(this.category.year, this.category.name);
            this.stateService.publishCategorySelected(this.category);
        }
    }

    private isMouseEventForThisInstance(intersections: Array<THREE.Intersection>): boolean {
        intersections = intersections.filter(x => x.object.parent instanceof CategoryVisual);

        if(intersections.length === 0) {
            return false;
        }

        for (let i = 0; i < intersections.length; i++) {
            if (intersections[i].object.parent instanceof CategoryVisual) {
                if (this.uuid === intersections[i].object.parent.uuid) {
                    return true;
                }
            }
        }

        return false;
    }

    private onMouseOver() {
        this._isMouseOver = true;
        this._backgroundMesh.material = new THREE.MeshLambertMaterial({ color: 255, side: THREE.DoubleSide });

        if (this._mouseOutTween != null) {
            this._mouseOutTween.stop();
            this._mouseOutTween = null;
        }

        this._mouseOverTween = new TWEEN.Tween(this.position)
            .to(this._hoverPosition, 1000)
            .easing(TWEEN.Easing.Elastic.Out)
            .start();
    }

    private onMouseOut() {
        this._isMouseOver = false;
        this._backgroundMesh.material = new THREE.MeshLambertMaterial({ color: this.color, side: THREE.DoubleSide });

        if (this._mouseOverTween != null) {
            this._mouseOverTween.stop();
            this._mouseOverTween = null;
        }

        this._mouseOutTween = new TWEEN.Tween(this.position)
            .to(this.onscreenPosition, 1200)
            .easing(TWEEN.Easing.Linear.None)
            .start();
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

        for (let faceIdx = 0; faceIdx < faceCount; ++faceIdx) {
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
