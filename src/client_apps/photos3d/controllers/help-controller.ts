import { HelpVisual } from '../visuals/help-visual';
import { IController } from './icontroller';

export class HelpController implements IController {
    private _help: HelpVisual;

    get areVisualsEnabled(): boolean {
        return this._help.isVisible;
    }

    init(): void {
        this._help = new HelpVisual();
        this._help.init();
    }

    render(): void {

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
}
