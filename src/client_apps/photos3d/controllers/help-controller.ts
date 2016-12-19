import { ArgumentNullError } from '../models/argument-null-error';
import { HelpVisual } from '../visuals/help-visual';
import { IController } from './icontroller';
import { StateService } from '../services/state-service';

export class HelpController implements IController {
    private _help: HelpVisual;

    private _disposed = false;

    constructor(private _stateService: StateService) {
        if (_stateService == null) {
            throw new ArgumentNullError('_stateService');
        }
    }

    get areVisualsEnabled(): boolean {
        return this._help.isVisible;
    }

    init(): void {
        this._help = new HelpVisual(this._stateService);
        this._help.init();
    }

    render(delta: number, elapsed: number): void {

    }

    enableVisuals(areEnabled: boolean): void {
        if (areEnabled !== this.areVisualsEnabled) {
            if (areEnabled) {
                this._help.show();
            } else {
                this._help.hide();
            }
        }
    }

    dispose(): void {
        if (!this._disposed) {
            this._disposed = true;

            this._help.dispose();
            this._help = null;
        }
    }
}
