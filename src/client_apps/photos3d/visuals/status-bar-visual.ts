import { IVisual } from './ivisual';
import { ArgumentNullError } from '../models/argument-null-error';
import { ActiveStatus } from '../models/active-status';
import { StatusBarController } from '../controllers/status-bar-controller';

export class StatusBarVisual implements IVisual {
    navDiv: HTMLDivElement;
    actualDiv: HTMLDivElement;
    temporalDiv: HTMLDivElement;
    statusDiv: HTMLDivElement;

    constructor(private _controller: StatusBarController) {
        if (_controller == null) {
            throw new ArgumentNullError('_controller');
        }
    }

    init() {
        this.navDiv = document.createElement('div');
        this.navDiv.id = 'nav';
        this.navDiv.style.position = 'absolute';
        this.navDiv.style.bottom = '0';
        this.navDiv.style.opacity = '0.7';
        this.navDiv.style.backgroundColor = '#333';
        this.navDiv.style.height = '24px';
        this.navDiv.style.width = '100%';
        this.navDiv.style.fontWeight = 'bold';

        document.body.appendChild(this.navDiv);

        this.actualDiv = this.createInfoDiv('33%', 'left');
        this.actualDiv.style.padding = '2px 8px';
        this.navDiv.appendChild(this.actualDiv);

        this.temporalDiv = this.createInfoDiv('33%', 'left');
        this.temporalDiv.style.padding = '2px 8px';
        this.temporalDiv.style.textAlign = 'center';
        this.navDiv.appendChild(this.temporalDiv);

        this.statusDiv = this.createInfoDiv('33%', 'right');
        this.statusDiv.style.padding = '2px 8px';
        this.navDiv.appendChild(this.statusDiv);
    }

    render() {

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
