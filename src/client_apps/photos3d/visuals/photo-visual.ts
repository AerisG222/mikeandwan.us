import { ArgumentNullError } from '../models/argument-null-error';
import { IPhoto } from '../models/iphoto';
import { IVisual } from './ivisual';
import { ScaleCalculator } from '../services/scale-calculator';
import { StateService } from '../services/state-service';
import { VisualContext } from '../models/visual-context';

export class PhotoVisual extends THREE.Object3D implements IVisual {
    private static readonly loader = new THREE.TextureLoader();

    private _ctx: VisualContext;

    constructor(private _photo: IPhoto,
                private _stateService: StateService,
                private _scaleCalculator: ScaleCalculator,
                private _height: number,
                private _width: number,
                private _z: number,) {
        super();

        if (_photo == null) {
            throw new ArgumentNullError('_photo');
        }

        if (_stateService == null) {
            throw new ArgumentNullError('_stateService');
        }

        if (_scaleCalculator == null) {
            throw new ArgumentNullError('_scaleCalculator');
        }

        this._ctx = this._stateService.visualContext;
    }

    init() {
        PhotoVisual.loader.load(this._photo.lgImage.path, texture => {
            this.createPhoto(texture);
        });

        this.position.z = this._z;
    }

    render() {

    }

    private createPhoto(texture: THREE.Texture): void {
        texture.minFilter = THREE.LinearFilter;

        let dimensions = this._scaleCalculator.scale(this._width, this._height, texture.image.width, texture.image.height);
        let plane = new THREE.PlaneGeometry(dimensions.x, dimensions.y);
        let material = new THREE.MeshBasicMaterial({ map: texture, side: THREE.DoubleSide });
        let mesh = new THREE.Mesh(plane, material);

        this.position.y = dimensions.y / 2;
        this.add(mesh);
    }
}
