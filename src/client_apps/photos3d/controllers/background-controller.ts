import { ArgumentNullError } from '../models/argument-null-error';
import { StateService } from '../services/state-service';
import { IController } from './icontroller';
import { IVisual } from '../visuals/ivisual';
import { PondBackgroundVisual } from '../visuals/pond-background-visual';

export class BackgroundController implements IController {
    private _visualsEnabled = true;
    private _background: IVisual;

    constructor(private _stateService: StateService) {
        if (_stateService == null) {
            throw new ArgumentNullError('_stateService');
        }
    }

    get areVisualsEnabled(): boolean {
        return this._visualsEnabled;
    }

    init(): void {
        // replace with factory if/when there are other background types
        this._background = new PondBackgroundVisual(this._stateService.visualContext);
        this._background.init();
    }

    render(clockDelta: number): void {
        if (this._visualsEnabled) {
            this._background.render(clockDelta);
        }
    }

    enableVisuals(areEnabled: boolean): void {

    }
}
