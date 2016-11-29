import { ArgumentNullError } from '../models/argument-null-error';
import { FrustrumCalculator } from '../services/frustrum-calculator';
import { IVisual } from './ivisual';
import { StateService } from '../services/state-service';

export class PhotoBackgroundVisual implements IVisual {
    private _bgMesh: THREE.Mesh;

    constructor(private _stateService: StateService,
                private _frustrumCalculator: FrustrumCalculator,
                private _z) {
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

    render(clockDelta: number) {

    }

    private createBackground(): void {
        let bounds = this._frustrumCalculator.calculateBounds(this._stateService.visualContext.camera, this._z);

        let plane = new THREE.PlaneGeometry(bounds.x, bounds.y);
        let material = new THREE.MeshBasicMaterial({ color: '#000000', transparent: true, opacity: 0.88, side: THREE.DoubleSide });
        this._bgMesh = new THREE.Mesh(plane, material);

        this._bgMesh.position.z = this._z;
        this._bgMesh.position.y = bounds.y / 2;

        this._stateService.visualContext.scene.add(this._bgMesh);
    }
}
