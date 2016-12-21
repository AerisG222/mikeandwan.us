import { Subscription } from 'rxjs/Subscription';

import { DisposalService } from '../services/disposal-service';
import { Hexagon } from '../models/hexagon';
import { ICategory } from '../models/icategory';
import { IVisual } from './ivisual';
import { StateService } from '../services/state-service';

export class CategoryVisual extends THREE.Object3D implements IVisual {
    private static readonly IMAGE_DEPTH = 4;
    private static readonly BORDER_WIDTH = 2;

    private static loader = new THREE.TextureLoader();

    private _disposed = false;
    private _ignoreMouseEvents = true;
    private _isMouseOver = false;
    private _backgroundMesh: THREE.Mesh = null;
    private _imageMesh: THREE.Mesh = null;
    private _hoverPosition: THREE.Vector3;
    private _bringIntoViewTween: TWEEN.Tween;
    private _removeFromViewTween: TWEEN.Tween;
    private _mouseOverSubscription: Subscription;
    private _mouseClickSubscription: Subscription;
    private _mouseOverTween: TWEEN.Tween;
    private _mouseOutTween: TWEEN.Tween;
    private _pauseSubscription: Subscription;
    private _rotateAnimationWaitTime: number;
    private _rotateNextTriggerTime: number;
    private _rotateStopTime: number;
    private _rotateDuration: number;
    private _rotateIsAnimating = false;

    constructor(private _stateService: StateService,
                private _disposalService: DisposalService,
                private _category: ICategory,
                private _hexagon: Hexagon,
                private _onscreenPosition: THREE.Vector3,
                private _offscreenPosition: THREE.Vector3,
                private _color: number) {
        super();

        this._rotateAnimationWaitTime = Math.random() * 110 + 10;
        this._rotateDuration = 1.0;
        this._rotateNextTriggerTime = this._rotateAnimationWaitTime;
        this._rotateStopTime = this._rotateDuration + this._rotateNextTriggerTime;

        this.position.set(_offscreenPosition.x, _offscreenPosition.y, _offscreenPosition.z);
        this._hoverPosition = new THREE.Vector3(_onscreenPosition.x, _onscreenPosition.y, _onscreenPosition.z + 12);
    }

    private get backgroundEdgeLength() {
        return this._hexagon.centerToVertexLength + CategoryVisual.BORDER_WIDTH;
    }

    private get imageEdgeLength() {
        return this._hexagon.centerToVertexLength;
    }

    init() {
        CategoryVisual.loader.load(this._category.teaserImage.path, texture => {
            this.createObject(texture);
        });

        this.createBackground();
        this.add(this._backgroundMesh);

        this._mouseOverSubscription = this._stateService.mouseoverObservable.subscribe(x => this.onMouseEvent(x));
        this._mouseClickSubscription = this._stateService.mouseclickObservable.subscribe(x => this.onMouseClick(x));
        this._pauseSubscription = this._stateService.pausedObservable.subscribe(x => this.onPause(x));
    }

    bringIntoView(): void {
        if (this._removeFromViewTween != null) {
            this._removeFromViewTween.stop();
            this._removeFromViewTween = null;
        }

        this._bringIntoViewTween = new TWEEN.Tween(this.position)
            .to(this._onscreenPosition, 1200)
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
            .to(this._offscreenPosition, 1200)
            .easing(TWEEN.Easing.Sinusoidal.Out)
            .start();
    }

    render(delta: number, elapsed: number) {
        if (this._disposed) {
            return;
        }

        if (this._rotateIsAnimating) {
            if (elapsed > this._rotateStopTime) {
                this.stopRotation();
            } else {
                let axis = new THREE.Vector3(Math.random() * 200, Math.random() * 200, Math.random() * 200);
                this._backgroundMesh.rotateOnAxis(axis.normalize(), Math.random() * 0.4);
            }
        } else {
            if (elapsed > this._rotateNextTriggerTime) {
                this._rotateIsAnimating = true;
                this.updateElapsedTime(elapsed);
            }
        }
    }

    dispose(): void {
        if (!this._disposed) {
            this._disposed = true;

            this._mouseClickSubscription.unsubscribe();
            this._mouseClickSubscription = null;

            this._mouseOverSubscription.unsubscribe();
            this._mouseOverSubscription = null;

            this._pauseSubscription.unsubscribe();
            this._pauseSubscription = null;

            this._disposalService.dispose(this);

            this._backgroundMesh = null;
            this._imageMesh = null;
        }
    }

    updateElapsedTime(elapsed: number): void {
        this._rotateNextTriggerTime = elapsed + this._rotateAnimationWaitTime;
        this._rotateStopTime = elapsed + this._rotateDuration;
    }

    private stopRotation() {
        this._rotateIsAnimating = false;
        this._backgroundMesh.rotation.set(0, 0, 0);
    }

    private onPause(isPaused: boolean) {
        if(isPaused) {
            if(this._rotateIsAnimating) {
                this.stopRotation();
            }

            // clock resets after pause
            this.updateElapsedTime(0);
        }
    }

    private createObject(texture: THREE.Texture) {
        this.createImage(texture);
        this.add(this._imageMesh);
    }

    private createBackground() {
        let geometry = this.createExtrudeGeometry(this.backgroundEdgeLength, this.imageEdgeLength + 1);
        let material = new THREE.MeshLambertMaterial({ color: this._color, side: THREE.DoubleSide });

        this._backgroundMesh = new THREE.Mesh(geometry, material);
    }

    private createImage(texture: THREE.Texture) {
        texture.minFilter = THREE.LinearFilter;

        let geometry = this.createExtrudeGeometry(this.imageEdgeLength, null);

        this.mapUvs(geometry);

        let material = new THREE.MeshLambertMaterial({ map: texture, side: THREE.DoubleSide });

        this._imageMesh = new THREE.Mesh(geometry, material);
    }

    private onMouseEvent(intersections: Array<THREE.Intersection>) {
        if (this._ignoreMouseEvents) {
            return;
        }

        let isMouseOver = this.isMouseEventForThisInstance(intersections);

        if (!isMouseOver) {
            if (this._isMouseOver) {
                this.onMouseOut();
            }
        } else {
            // TODO: we currently clear the temporal status on a mouse event, expecting a hovered category to set this
            //       seems like there might be a more performant option
            this._stateService.publishTemporalNav(this._category.name);

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

        if (isMouseOver) {
            this._stateService.publishTemporalNav(null);
            this._stateService.publishActiveNav(this._category.year, this._category.name);
            this._stateService.publishCategorySelected(this._category);
        }
    }

    private isMouseEventForThisInstance(intersections: Array<THREE.Intersection>): boolean {
        intersections = intersections.filter(x => x.object.parent instanceof CategoryVisual);

        if (intersections.length === 0) {
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
        this._backgroundMesh.material = new THREE.MeshLambertMaterial({ color: this._color, side: THREE.DoubleSide });

        if (this._mouseOverTween != null) {
            this._mouseOverTween.stop();
            this._mouseOverTween = null;
        }

        this._mouseOutTween = new TWEEN.Tween(this.position)
            .to(this._onscreenPosition, 1200)
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

    private createExtrudeGeometry(outerLength: number, innerLength?: number): THREE.ExtrudeGeometry {
        let hex = new Hexagon(outerLength);
        let shape = new THREE.Shape(hex.generatePoints());

        if (innerLength != null) {
            let hexHole = new Hexagon(innerLength);
            shape.holes.push(new THREE.Path(hexHole.generatePoints()));
        }

        let settings = {
            amount: CategoryVisual.IMAGE_DEPTH,
            bevelEnabled: false
        };

        return new THREE.ExtrudeGeometry(shape, settings);
    }
}
