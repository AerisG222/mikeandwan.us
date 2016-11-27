import { DataService } from '../services/data-service';
import { IController } from './icontroller';
import { StateService } from '../services/state-service';

export class PhotoListController implements IController {
    private _visualsEnabled = true;

    constructor(private _dataService: DataService,
                private _stateService: StateService) {
        
    }

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
