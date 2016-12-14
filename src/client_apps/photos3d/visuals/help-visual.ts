import { ArgumentNullError } from '../models/argument-null-error';
import { IVisual } from './ivisual';
import { StateService } from '../services/state-service';

export class HelpVisual implements IVisual {
    private static readonly _modalSelector = '#help';

    private _isVisible = false;
    private _modal: JQuery;

    constructor(private _stateService: StateService) {
        if (_stateService == null) {
            throw new ArgumentNullError('_stateService');
        }
    }

    get isVisible() {
        return this._isVisible;
    }

    init() {
        this._modal = jQuery(HelpVisual._modalSelector);

        this._modal.on('show.bs.modal', e => {
            this._stateService.publishDialogDisplayed(true);
        });

        this._modal.on('shown.bs.modal', e => {
            this._isVisible = true;
        });

        this._modal.on('hidden.bs.modal', e => {
            this._stateService.publishDialogDisplayed(false);
            this._isVisible = false;
        });
    }

    render(clockDelta: number, elapsed: number) {

    }

    show() {
        this._modal.modal('show');
    }

    hide() {
        this._modal.modal('hide');
    }
}
