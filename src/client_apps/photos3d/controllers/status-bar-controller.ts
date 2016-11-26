import { ArgumentNullError } from '../models/argument-null-error';
import { IController } from './icontroller';
import { StateService } from '../services/state-service';
import { StatusBarVisual } from '../visuals/status-bar-visual';

export class StatusBarController implements IController {
    private _visualsEnabled = true;
    private _status: StatusBarVisual;

    constructor(private _stateService: StateService) {
        if(_stateService == null) {
            throw new ArgumentNullError('_stateService');
        }
    }

    get areVisualsEnabled(): boolean {
        return this._visualsEnabled;
    }

    init(): void {
        // replace with factory if/when there are other background types
        this._status = new StatusBarVisual(this);
        this._status.init();
        
        this._stateService.activeNavObservable.subscribe(evt => { this._status.updateActive(evt); });
        this._stateService.temporalNavObservable.subscribe(evt => { this._status.updateTemporal(evt); });
    }

    render(): void {
        
    }

    enableVisuals(areEnabled: boolean): void {

    }
}
