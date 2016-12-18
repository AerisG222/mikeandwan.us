import { ArgumentNullError } from '../models/argument-null-error';
import { DisposalService } from '../services/disposal-service';
import { IController } from './icontroller';
import { IVisual } from '../visuals/ivisual';
import { PondBackgroundVisual } from '../visuals/pond-background-visual';
import { StateService } from '../services/state-service';

export class BackgroundController implements IController {
    private _visualsEnabled = true;
    private _background: IVisual;

    constructor(private _stateService: StateService,
                private _disposalService: DisposalService) {
        if (_stateService == null) {
            throw new ArgumentNullError('_stateService');
        }

        if (_disposalService == null) {
            throw new ArgumentNullError('_disposalService');
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

    render(clockDelta: number, elapsed: number): void {
        if (this._visualsEnabled) {
            this._background.render(clockDelta, elapsed);
        }
    }

    enableVisuals(areEnabled: boolean): void {

    }
}
