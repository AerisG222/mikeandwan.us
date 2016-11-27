import { ArgumentNullError } from '../models/argument-null-error';
import { IPhoto } from '../models/iphoto';
import { IVisual } from './ivisual';
import { StateService } from '../services/state-service';
import { VisualContext } from '../models/visual-context';

export class PhotoVisual extends THREE.Object3D implements IVisual {
    private static loader = new THREE.TextureLoader();
    
    private _ctx: VisualContext;

    constructor(private _photo: IPhoto,
                private _stateService: StateService) {
        super();

        if(_photo == null) {
            throw new ArgumentNullError('_photo');
        }

        if(_stateService == null) {
            throw new ArgumentNullError('_stateService');
        }

        this._ctx = this._stateService.visualContext;
    }

    init() {
        PhotoVisual.loader.load(this._photo.lgImage.path, texture => {
            this.createPhoto(texture);
        });

        this.position.z = 500;// this._ctx.camera.position.z - 1; // put it right in front of the camera
    }

    render() {

    }

    private createPhoto(texture: THREE.Texture): void {
        let plane = new THREE.PlaneGeometry(texture.image.width, texture.image.height);
        let material = new THREE.MeshBasicMaterial({ map: texture, side: THREE.DoubleSide });
        let mesh = new THREE.Mesh(plane, material);

        this.add(mesh);
    }
}
