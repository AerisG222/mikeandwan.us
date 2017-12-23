import { Scene, PerspectiveCamera, WebGLRenderer, DirectionalLight, AmbientLight } from 'three';

import { ArgumentNullError } from './argument-null-error';
import { DisposalService } from '../services/disposal-service';
import { IDisposable } from './idisposable';

export class VisualContext implements IDisposable {
    private _isDisposed = false;

    constructor(private _disposalService: DisposalService,
                private _scene?: Scene,
                private _camera?: PerspectiveCamera,
                private _renderer?: WebGLRenderer,
                private _sun?: DirectionalLight,
                private _ambient?: AmbientLight) {
        if (_disposalService == null) {
            throw new ArgumentNullError('_disposalService');
        }
    }

    get scene() {
        return this._scene;
    }

    set scene(scene: Scene) {
        if (scene == null) {
            throw new ArgumentNullError('scene');
        }

        this._scene = scene;
    }

    get camera() {
        return this._camera;
    }

    set camera(camera: PerspectiveCamera) {
        if (camera == null) {
            throw new ArgumentNullError('camera');
        }

        this._camera = camera;
    }

    get renderer() {
        return this._renderer;
    }

    set renderer(renderer: WebGLRenderer) {
        if (renderer == null) {
            throw new ArgumentNullError('renderer');
        }

        this._renderer = renderer;
    }

    get sun() {
        return this._sun;
    }

    set sun(sun: DirectionalLight) {
        if (sun == null) {
            throw new ArgumentNullError('sun');
        }

        this._sun = sun;
    }

    get ambient() {
        return this._ambient;
    }

    set ambient(ambient: AmbientLight) {
        if (ambient == null) {
            throw new ArgumentNullError('ambient');
        }

        this._ambient = ambient;
    }

    get height() {
        return window.innerHeight;
    }

    get width() {
        return window.innerWidth;
    }

    get size() {
        if (this.width < 2200) {
            return 'md';
        } else {
            return 'lg';
        }
    }

    dispose(): void {
        if (this._isDisposed) {
            return;
        }

        this._isDisposed = true;

        this.scene.remove(this._sun);
        this._disposalService.dispose(this._sun);
        this._sun = null;

        this.scene.remove(this._ambient);
        this._disposalService.dispose(this._ambient);
        this._ambient = null;

        this.scene.remove(this._camera);
        this._disposalService.dispose(this._camera);
        this._camera = null;

        this._disposalService.dispose(this._scene);
        this._scene = null;

        this._renderer.dispose();
        this._renderer = null;
    }
}
