import { IController } from './icontroller';

export class PhotoListController implements IController {
    private _visualsEnabled = true;

    get areVisualsEnabled(): boolean {
        return this._visualsEnabled;
    }

    init(): void {

    }

    render(): void {

    }

    enableVisuals(areEnabled: boolean): void {

    }
}
