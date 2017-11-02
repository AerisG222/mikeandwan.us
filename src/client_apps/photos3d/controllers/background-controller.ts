import { ArgumentNullError } from '../models/argument-null-error';
import { DisposalService } from '../services/disposal-service';
import { IController } from './icontroller';
import { IDisposable } from '../models/idisposable';
import { PondBackgroundVisual } from '../visuals/pond-background-visual';
import { StateService } from '../services/state-service';

export class BackgroundController implements IController, IDisposable {
    private _background: PondBackgroundVisual;
    private _isDisposed = false;
    private _visualsEnabled = true;

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
        this._background = new PondBackgroundVisual(this._disposalService, this._stateService.visualContext);
        this._background.init();
        this._stateService.visualContext.scene.add(this._background);
    }

    render(clockDelta: number, elapsed: number): void {
        if (this._isDisposed) {
            return;
        }

        if (this._visualsEnabled) {
            this._background.render(clockDelta, elapsed);
        }
    }

    enableVisuals(areEnabled: boolean): void {

    }

    dispose(): void {
        if (this._isDisposed) {
            return;
        }

        this._isDisposed = true;
        this._stateService.visualContext.scene.remove(this._background);
        this._disposalService.dispose(this._background);
        this._background = null;
    }
}
