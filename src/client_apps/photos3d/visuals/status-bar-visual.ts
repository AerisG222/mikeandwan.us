import { IVisual } from './ivisual';
import { ArgumentNullError } from '../models/argument-null-error';
import { ActiveStatus } from '../models/active-status';
import { StatusBarController } from '../controllers/status-bar-controller';

export class StatusBarVisual implements IVisual {
    private _rootDiv: HTMLDivElement;
    private _actualDiv: HTMLDivElement;
    private _temporalDiv: HTMLDivElement;
    private _statusDiv: HTMLDivElement;
    private _helpButton: HTMLButtonElement;

    constructor(private _controller: StatusBarController) {
        if (_controller == null) {
            throw new ArgumentNullError('_controller');
        }
    }

    init() {
        this._rootDiv = document.createElement('div');
        this._rootDiv.id = 'nav';
        this._rootDiv.style.position = 'absolute';
        this._rootDiv.style.bottom = '0';
        this._rootDiv.style.opacity = '0.7';
        this._rootDiv.style.backgroundColor = '#333';
        this._rootDiv.style.height = '24px';
        this._rootDiv.style.width = '100%';
        this._rootDiv.style.fontWeight = 'bold';

        document.body.appendChild(this._rootDiv);

        this._actualDiv = this.createInfoDiv('33.333333%', '2px 8px');
        this._rootDiv.appendChild(this._actualDiv);

        this._temporalDiv = this.createInfoDiv('33.333333%', '2px 8px', 'center');
        this._rootDiv.appendChild(this._temporalDiv);

        this._statusDiv = this.createInfoDiv('33.333333%', '0 8px', 'right');
        this._rootDiv.appendChild(this._statusDiv);

        this._helpButton = document.createElement('button');
        this._helpButton.type = 'button';
        this._helpButton.classList.add('btn', 'btn-primary', 'btn-xs');
        this._helpButton.setAttribute('data-toggle', 'modal');
        this._helpButton.setAttribute('data-target', '#help');
        this._helpButton.innerText = '?';
        this._helpButton.style.fontWeight = 'bold';
        this._statusDiv.appendChild(this._helpButton);
    }

    render(clockDelta: number, elapsed: number) {

    }

    dispose(): void {

    }

    show() {
        this._rootDiv.style.display = 'block';
    }

    hide() {
        this._rootDiv.style.display = 'none';
    }

    updateTemporal(category: string) {
        this._temporalDiv.innerHTML = category;
    }

    updateActive(status: ActiveStatus) {
        if (status.category != null) {
            this._actualDiv.innerHTML = `${status.year} | ${status.category}`;
        } else {
            this._actualDiv.innerHTML = status.year.toString();
        }
    }

    updateStatus() {
        // TODO: anything we want to show here?  fps? help/keybindings? exit? about?
    }

    private createInfoDiv(width: string, padding?: string, textAlign?: string) {
        let div = document.createElement('div');

        div.style.width = width;
        div.style.cssFloat = 'left';

        if (padding != null) {
            div.style.padding = padding;
        }

        if (textAlign != null) {
            div.style.textAlign = textAlign;
        }

        return div;
    }
}
