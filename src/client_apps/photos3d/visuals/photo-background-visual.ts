import { ArgumentNullError } from '../models/argument-null-error';
import { FrustrumCalculator } from '../services/frustrum-calculator';
import { IVisual } from './ivisual';
import { StateService } from '../services/state-service';

export class PhotoBackgroundVisual extends THREE.Object3D implements IVisual {
    constructor(private _stateService: StateService,
                private _frustrumCalculator: FrustrumCalculator,
                private _z) {
        super();

        if (_stateService == null) {
            throw new ArgumentNullError('_stateService');
        }

        if (_frustrumCalculator == null) {
            throw new ArgumentNullError('_frustrumCalculator');
        }
    }

    init() {
        this.createBackground();
    }

    render(clockDelta: number, elapsed: number) {

    }

    dispose(): void {

    }

    private createBackground(): void {
        let bounds = this._frustrumCalculator.calculateBounds(this._stateService.visualContext.camera, this._z);

        let plane = new THREE.PlaneGeometry(bounds.x, bounds.y);
        let material = new THREE.MeshBasicMaterial({ color: '#000000', transparent: true, opacity: 0.88, side: THREE.DoubleSide });
        let mesh = new THREE.Mesh(plane, material);

        mesh.position.z = this._z;
        mesh.position.y = bounds.y / 2;

        this.add(mesh);
    }
}
