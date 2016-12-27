import { ArgumentNullError } from '../models/argument-null-error';
import { DisposalService } from '../services/disposal-service';
import { FrustrumCalculator } from '../services/frustrum-calculator';
import { IDisposable } from '../models/idisposable';
import { IVisual } from './ivisual';
import { StateService } from '../services/state-service';

export class PhotoBackgroundVisual extends THREE.Object3D implements IDisposable, IVisual {
    private _isDisposed = false;
    private _photoMesh: THREE.Mesh = null;

    constructor(private _stateService: StateService,
                private _disposalService: DisposalService,
                private _frustrumCalculator: FrustrumCalculator,
                private _z) {
        super();

        if (_stateService == null) {
            throw new ArgumentNullError('_stateService');
        }

        if (_frustrumCalculator == null) {
            throw new ArgumentNullError('_frustrumCalculator');
        }

        if (_disposalService == null) {
            throw new ArgumentNullError('_disposalService');
        }
    }

    init() {
        this.createBackground();
    }

    render(clockDelta: number, elapsed: number) {

    }

    dispose(): void {
        if(this._isDisposed) {
            return;
        }

        this._isDisposed = true;

        this.remove(this._photoMesh);
        this._disposalService.dispose(this._photoMesh);
        this._photoMesh = null;
    }

    private createBackground(): void {
        let bounds = this._frustrumCalculator.calculateBounds(this._stateService.visualContext.camera, this._z);

        let plane = new THREE.PlaneGeometry(bounds.x, bounds.y);
        let material = new THREE.MeshPhongMaterial({ color: '#000000', transparent: true, opacity: 0.88, side: THREE.DoubleSide });
        this._photoMesh = new THREE.Mesh(plane, material);

        this._photoMesh.position.z = this._z;
        this._photoMesh.position.y = bounds.y / 2;

        this.add(this._photoMesh);
    }
}
