import { IVisual } from './ivisual';

export class HelpVisual implements IVisual {
    private static readonly _modalSelector = '#help';

    private _isVisible = false;
    private _modal: JQuery;

    get isVisible() {
        return this._isVisible;
    }

    init() {
        this._modal = jQuery(HelpVisual._modalSelector);

        this._modal.on('hidden.bs.modal', e => {
            this._isVisible = false;
        });

        this._modal.on('shown.bs.modal', e => {
            this._isVisible = true;
        });
    }

    render(clockDelta: number) {

    }

    show() {
        this._modal.modal('show');
    }

    hide() {
        this._modal.modal('hide');
    }
}
