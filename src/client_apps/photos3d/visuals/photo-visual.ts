import { ArgumentNullError } from '../models/argument-null-error';
import { DisposalService } from '../services/disposal-service';
import { IDisposable } from '../models/idisposable';
import { IPhoto } from '../models/iphoto';
import { IVisual } from './ivisual';
import { ScaleCalculator } from '../services/scale-calculator';
import { StateService } from '../services/state-service';
import { VisualContext } from '../models/visual-context';

export class PhotoVisual extends THREE.Object3D implements IDisposable, IVisual {
    private static readonly loader = new THREE.TextureLoader();

    private _isDisposed = false;
    private _rotateOutDirection = 0;

    private _ctx: VisualContext;
    private _mesh: THREE.Mesh;
    private _rotationAnchor: THREE.Object3D;

    constructor(private _photo: IPhoto,
                private _stateService: StateService,
                private _disposalService: DisposalService,
                private _scaleCalculator: ScaleCalculator,
                private _height: number,
                private _width: number,
                private _z: number) {
        super();

        if (_photo == null) {
            throw new ArgumentNullError('_photo');
        }

        if (_stateService == null) {
            throw new ArgumentNullError('_stateService');
        }

        if (_disposalService == null) {
            throw new ArgumentNullError('_disposalService');
        }

        if (_scaleCalculator == null) {
            throw new ArgumentNullError('_scaleCalculator');
        }

        this._ctx = this._stateService.visualContext;

        this._rotationAnchor = new THREE.Object3D();
        this.add(this._rotationAnchor);
    }

    get isHidden() {
        return this._isDisposed || this._mesh == null || this._mesh.material.opacity <= 0.0;
    }

    init() {
        PhotoVisual.loader.load(this.getPhotoUrl(), texture => {
            this.createPhoto(texture);
        });
    }

    render(clockDelta: number, elapsed: number) {
        if (this._isDisposed || this._mesh == null) {
            return;
        }

        if (this._rotateOutDirection !== 0.0) {
            this._mesh.position.z -= 2;
            this._mesh.material.opacity -= 0.02;

            this._rotationAnchor.rotateY(this._rotateOutDirection * Math.PI / 100);
        }
    }

    dispose(): void {
        if (this._isDisposed) {
            return;
        }

        this._isDisposed = true;

        this._rotationAnchor.remove(this._mesh);
        this._disposalService.dispose(this._mesh);
        this._mesh = null;

        this.remove(this._rotationAnchor);
        this._disposalService.dispose(this._rotationAnchor);
        this._rotationAnchor = null;
    }

    hide(direction: number) {
        this._rotateOutDirection = direction;
    }

    private getPhotoUrl(): string {
        if (this._width < 1600 || this._height < 1200) {
            return this._photo.mdImage.path;
        }

        return this._photo.lgImage.path;
    }

    private createPhoto(texture: THREE.Texture): void {
        texture.minFilter = THREE.LinearFilter;

        let dimensions = this._scaleCalculator.scale(this._width, this._height, texture.image.width, texture.image.height);
        let plane = new THREE.PlaneGeometry(dimensions.x, dimensions.y);
        let material = new THREE.MeshBasicMaterial({ map: texture, side: THREE.DoubleSide });
        this._mesh = new THREE.Mesh(plane, material);

        this._mesh.position.y = dimensions.y / 2;
        this._mesh.position.z = this._z;
        this._rotationAnchor.add(this._mesh);
    }
}
