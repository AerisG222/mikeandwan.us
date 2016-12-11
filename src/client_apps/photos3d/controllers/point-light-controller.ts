import { ArgumentNullError } from '../models/argument-null-error';
import { FrustrumCalculator } from '../services/frustrum-calculator';
import { IController } from './icontroller';
import { StateService } from '../services/state-service';

export class PointLightController implements IController {
    private static readonly NUM_LIGHTS = 3;
    private static readonly COLORS = [ 0xff3333, 0x33ff33, 0x3333ff ];

    private _visualsEnabled = false;
    private _lights: Array<THREE.PointLight> = [];
    private _minX: number;
    private _maxX: number;

    constructor(private _stateService: StateService,
                private _frustrumCalculator: FrustrumCalculator) {
        if (_stateService == null) {
            throw new ArgumentNullError('_stateService');
        }

        if (_frustrumCalculator == null) {
            throw new ArgumentNullError('_frustrumCalculator');
        }

        let width = this._frustrumCalculator.calculateBounds(this._stateService.visualContext.camera, 510).x;
        this._minX = -width / 2.0;
        this._maxX = width / 2.0;
    }

    get areVisualsEnabled(): boolean {
        return this._visualsEnabled;
    }

    init(): void {
        let sphere = new THREE.SphereGeometry( 4, 16, 8 );

        for (let i = 0; i < PointLightController.NUM_LIGHTS; i++) {
            let color = PointLightController.COLORS[i];
            let light = new THREE.PointLight(color, 1, 100);

            light.add(new THREE.Mesh(sphere, new THREE.MeshBasicMaterial( { color: color } )));

            light.userData = {
                speed: Math.random() * 2 + 2,
                direction: (Math.random() * 100) % 2 === 0 ? 1 : -1
            };

            light.position.y = Math.random() * 200 + 150;
            light.position.x = Math.random() * (this._maxX * 2) - this._maxX;
            light.position.z = 510;

            this._lights.push(light);
        }
    }

    enableVisuals(areEnabled: boolean): void {
        if (areEnabled === this.areVisualsEnabled) {
            return;
        }

        if (areEnabled) {
            for (let i = 0; i < PointLightController.NUM_LIGHTS; i++) {
                this._stateService.visualContext.scene.add(this._lights[i]);
            }
        } else {
            for (let i = this._lights.length - 1; i >= 0; i--) {
                this._stateService.visualContext.scene.remove(this._lights[i]);
            }
        }

        this._visualsEnabled = areEnabled;
    }

    render(clockDelta: number): void {
        if (this.areVisualsEnabled) {
            for (let i = 0; i < this._lights.length; i++) {
                this.updateLight(this._lights[i]);
            }
        }
    }

    private updateLight(light: THREE.PointLight): void {
        light.position.x += light.userData.direction * light.userData.speed;

        if (light.position.x > this._maxX) {
            light.position.x = this._maxX;
            light.userData.direction = -1;
        } else if (light.position.x < this._minX) {
            light.position.x = this._minX;
            light.userData.direction = 1;
        }
    }
}
