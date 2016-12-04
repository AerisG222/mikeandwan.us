import { ArgumentNullError } from './argument-null-error';

export class VisualContext {
    constructor(private _scene?: THREE.Scene,
                private _camera?: THREE.PerspectiveCamera,
                private _renderer?: THREE.Renderer,
                private _sun?: THREE.DirectionalLight,
                private _ambient?: THREE.AmbientLight) {
    }

    get scene() {
        return this._scene;
    }

    set scene(scene: THREE.Scene) {
        if (scene == null) {
            throw new ArgumentNullError('scene');
        }

        this._scene = scene;
    }

    get camera() {
        return this._camera;
    }

    set camera(camera: THREE.PerspectiveCamera) {
        if (camera == null) {
            throw new ArgumentNullError('camera');
        }

        this._camera = camera;
    }

    get renderer() {
        return this._renderer;
    }

    set renderer(renderer: THREE.Renderer) {
        if (renderer == null) {
            throw new ArgumentNullError('renderer');
        }

        this._renderer = renderer;
    }

    get sun() {
        return this._sun;
    }

    set sun(sun: THREE.DirectionalLight) {
        if (sun == null) {
            throw new ArgumentNullError('sun');
        }

        this._sun = sun;
    }

    get ambient() {
        return this._ambient;
    }

    set ambient(ambient: THREE.AmbientLight) {
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
}
