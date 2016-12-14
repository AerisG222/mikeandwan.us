import { IVisual } from './ivisual';
import { ArgumentNullError } from '../models/argument-null-error';
import { ActiveStatus } from '../models/active-status';
import { StatusBarController } from '../controllers/status-bar-controller';

export class StatusBarVisual implements IVisual {
    rootDiv: HTMLDivElement;
    actualDiv: HTMLDivElement;
    temporalDiv: HTMLDivElement;
    statusDiv: HTMLDivElement;

    constructor(private _controller: StatusBarController) {
        if (_controller == null) {
            throw new ArgumentNullError('_controller');
        }
    }

    init() {
        this.rootDiv = document.createElement('div');
        this.rootDiv.id = 'nav';
        this.rootDiv.style.position = 'absolute';
        this.rootDiv.style.bottom = '0';
        this.rootDiv.style.opacity = '0.7';
        this.rootDiv.style.backgroundColor = '#333';
        this.rootDiv.style.height = '24px';
        this.rootDiv.style.width = '100%';
        this.rootDiv.style.fontWeight = 'bold';

        document.body.appendChild(this.rootDiv);

        this.actualDiv = this.createInfoDiv('33%', 'left');
        this.actualDiv.style.padding = '2px 8px';
        this.rootDiv.appendChild(this.actualDiv);

        this.temporalDiv = this.createInfoDiv('33%', 'left');
        this.temporalDiv.style.padding = '2px 8px';
        this.temporalDiv.style.textAlign = 'center';
        this.rootDiv.appendChild(this.temporalDiv);

        this.statusDiv = this.createInfoDiv('33%', 'right');
        this.statusDiv.style.padding = '2px 8px';
        this.rootDiv.appendChild(this.statusDiv);
    }

    render(clockDelta: number, elapsed: number) {

    }

    show() {
        this.rootDiv.style.display = 'block';
    }

    hide() {
        this.rootDiv.style.display = 'none';
    }

    updateTemporal(category: string) {
        this.temporalDiv.innerHTML = category;
    }

    updateActive(status: ActiveStatus) {
        if (status.category != null) {
            this.actualDiv.innerHTML = `${status.year} | ${status.category}`;
        } else {
            this.actualDiv.innerHTML = status.year.toString();
        }
    }

    updateStatus() {
        // TODO: anything we want to show here?  fps? help/keybindings? exit? about?
    }

    private createInfoDiv(width: string, align: string) {
        let div = document.createElement('div');

        div.style.width = width;
        div.style.cssFloat = 'left';

        return div;
    }
}
