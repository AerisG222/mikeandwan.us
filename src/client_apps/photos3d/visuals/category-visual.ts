import { Subscription } from 'rxjs/Subscription';

import { Group, TextureLoader, Mesh, MeshLambertMaterial, DoubleSide, Vector3, Vector2,
         Shape, Path, ExtrudeGeometry, LinearFilter, Geometry, Texture
       } from 'three';

import { Tween, Easing } from 'tween.js';

import { DisposalService } from '../services/disposal-service';
import { Hexagon } from '../models/hexagon';
import { ICategory } from '../models/icategory';
import { IDisposable } from '../models/idisposable';
import { IMouseClickReceiver } from './imouse-click-reciever';
import { IMouseOverReceiver } from './imouse-over-reciever';
import { IVisual } from './ivisual';
import { MouseWatcherEvent } from '../models/mouse-watcher-event';
import { StateService } from '../services/state-service';

export class CategoryVisual extends Group implements IDisposable, IMouseClickReceiver, IMouseOverReceiver, IVisual {
    private static readonly IMAGE_DEPTH = 4;
    private static readonly BORDER_WIDTH = 2;
    private static readonly loader = new TextureLoader();

    private _backgroundMesh: Mesh = null;
    private _bringIntoViewTween: Tween;
    private _hoverPosition: Vector3;
    private _ignoreMouseEvents = true;
    private _imageMesh: Mesh = null;
    private _isDisposed = false;
    private _isMouseOver = false;
    private _mouseOutTween: Tween;
    private _mouseOverTween: Tween;
    private _pauseSubscription: Subscription;
    private _removeFromViewTween: Tween;
    private _rotateAnimationWaitTime: number;
    private _rotateDuration: number;
    private _rotateIsAnimating = false;
    private _rotateNextTriggerTime: number;
    private _rotateStopTime: number;

    constructor(private _stateService: StateService,
                private _disposalService: DisposalService,
                private _category: ICategory,
                private _hexagon: Hexagon,
                private _onscreenPosition: Vector3,
                private _offscreenPosition: Vector3,
                private _color: number) {
        super();

        this._rotateAnimationWaitTime = Math.random() * 110 + 10;
        this._rotateDuration = 1.0;
        this._rotateNextTriggerTime = this._rotateAnimationWaitTime;
        this._rotateStopTime = this._rotateDuration + this._rotateNextTriggerTime;

        this.position.set(_offscreenPosition.x, _offscreenPosition.y, _offscreenPosition.z);
        this._hoverPosition = new Vector3(_onscreenPosition.x, _onscreenPosition.y, _onscreenPosition.z + 12);
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

        this._pauseSubscription = this._stateService.pausedObservable.subscribe(x => this.onPause(x));
    }

    bringIntoView(): void {
        if (this._removeFromViewTween != null) {
            this._removeFromViewTween.stop();
            this._removeFromViewTween = null;
        }

        this._bringIntoViewTween = new Tween(this.position)
            .to(this._onscreenPosition, 1200)
            .easing(Easing.Back.Out)
            .start()
            .onComplete(x => { this._ignoreMouseEvents = false; });
    }

    removeFromView(): void {
        this._ignoreMouseEvents = true;

        if (this._bringIntoViewTween != null) {
            this._bringIntoViewTween.stop();
            this._bringIntoViewTween = null;
        }

        this._removeFromViewTween = new Tween(this.position)
            .to(this._offscreenPosition, 1200)
            .easing(Easing.Sinusoidal.Out)
            .start();
    }

    render(clockDelta: number, elapsed: number) {
        if (this._isDisposed) {
            return;
        }

        if (this._rotateIsAnimating) {
            if (elapsed > this._rotateStopTime) {
                this.stopRotation();
            } else {
                let axis = new Vector3(Math.random() * 200, Math.random() * 200, Math.random() * 200);
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
        if (this._isDisposed) {
            return;
        }

        this._isDisposed = true;

        this._pauseSubscription.unsubscribe();
        this._pauseSubscription = null;

        this.remove(this._backgroundMesh);
        this._disposalService.dispose(this._backgroundMesh);
        this._backgroundMesh = null;

        this.remove(this._imageMesh);
        this._disposalService.dispose(this._imageMesh);
        this._imageMesh = null;
    }

    updateElapsedTime(elapsed: number): void {
        this._rotateNextTriggerTime = elapsed + this._rotateAnimationWaitTime;
        this._rotateStopTime = elapsed + this._rotateDuration;
    }

    onMouseClick(evt: MouseWatcherEvent): void {
        evt.watcher.ignoreMouseEvents = true;

        this._stateService.publishTemporalNav(null);
        this._stateService.publishActiveNav(this._category.year, this._category.name);
        this._stateService.publishCategorySelected(this._category);
    }

    onMouseOver(evt: MouseWatcherEvent) {
        this._isMouseOver = true;
        this._backgroundMesh.material = new MeshLambertMaterial({ color: 255, side: DoubleSide });

        if (this._mouseOutTween != null) {
            this._mouseOutTween.stop();
            this._mouseOutTween = null;
        }

        this._mouseOverTween = new Tween(this.position)
            .to(this._hoverPosition, 1000)
            .easing(Easing.Elastic.Out)
            .start();

        this._stateService.publishTemporalNav(this._category.name);
    }

    onMouseOut(evt: MouseWatcherEvent) {
        this._isMouseOver = false;
        this._backgroundMesh.material = new MeshLambertMaterial({ color: this._color, side: DoubleSide });

        if (this._mouseOverTween != null) {
            this._mouseOverTween.stop();
            this._mouseOverTween = null;
        }

        this._mouseOutTween = new Tween(this.position)
            .to(this._onscreenPosition, 1200)
            .easing(Easing.Linear.None)
            .start();

        this._stateService.publishTemporalNav(null);
    }

    private stopRotation() {
        this._rotateIsAnimating = false;
        this._backgroundMesh.rotation.set(0, 0, 0);
    }

    private onPause(isPaused: boolean) {
        if (isPaused) {
            if (this._rotateIsAnimating) {
                this.stopRotation();
            }

            // clock resets after pause
            this.updateElapsedTime(0);
        }
    }

    private createObject(texture: Texture) {
        this.createImage(texture);
        this.add(this._imageMesh);
    }

    private createBackground() {
        let geometry = this.createExtrudeGeometry(this.backgroundEdgeLength, this.imageEdgeLength + 1);
        let material = new MeshLambertMaterial({ color: this._color, side: DoubleSide });

        this._backgroundMesh = new Mesh(geometry, material);
    }

    private createImage(texture: Texture) {
        texture.minFilter = LinearFilter;

        let geometry = this.createExtrudeGeometry(this.imageEdgeLength, null);

        this.mapUvs(geometry);

        let material = new MeshLambertMaterial({ map: texture, side: DoubleSide });

        this._imageMesh = new Mesh(geometry, material);
    }

    // https://github.com/mrdoob/js/issues/2065
    private mapUvs(geometry: Geometry) {
        geometry.computeBoundingBox();
        let minX = geometry.boundingBox.min.x;
        let minY = geometry.boundingBox.min.y;
        let maxX = geometry.boundingBox.max.x;
        let maxY = geometry.boundingBox.max.y;
        let deltaX = maxX - minX;
        let deltaY = maxY - minY;

        let faceCount = geometry.faces.length;
        let uvA = new Vector2(0, 0);
        let uvB = new Vector2(0, 0);
        let uvC = new Vector2(0, 0);

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

    private createExtrudeGeometry(outerLength: number, innerLength?: number): ExtrudeGeometry {
        let hex = new Hexagon(outerLength);
        let shape = new Shape(hex.generatePoints());

        if (innerLength != null) {
            let hexHole = new Hexagon(innerLength);
            shape.holes.push(new Path(hexHole.generatePoints()));
        }

        let settings = {
            amount: CategoryVisual.IMAGE_DEPTH,
            bevelEnabled: false
        };

        return new ExtrudeGeometry(shape, settings);
    }
}
