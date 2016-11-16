import { StateService } from './state-service';
import { ActiveStatus } from './active-status';

export class StatusBar {
    navDiv: HTMLDivElement;
    actualDiv: HTMLDivElement;
    temporalDiv: HTMLDivElement;
    statusDiv: HTMLDivElement;
    
    constructor(private stateService: StateService) {
        if(stateService == null) {
            throw new Error("stateService is not defined!");
        }

        this.stateService.ActiveNavObservable.subscribe(evt => { this.updateActive(evt); });
        this.stateService.TemporalNavObservable.subscribe(evt => { this.updateTemporal(evt); });
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

        document.body.appendChild(this.navDiv);

        this.actualDiv = this.createInfoDiv('33%', 'left');
        this.navDiv.appendChild(this.actualDiv);

        this.temporalDiv = this.createInfoDiv('33%', 'left')
        this.navDiv.appendChild(this.temporalDiv);

        this.statusDiv = this.createInfoDiv('33%', 'right');
        this.navDiv.appendChild(this.statusDiv);
    }

    private createInfoDiv(width: string, align: string) {
        let div = document.createElement('div');

        div.style.width = width;
        div.style.cssFloat = 'left';

        return div;
    }

    private updateTemporal(category: string) {
        this.temporalDiv.innerHTML = category;
    }

    private updateActive(status: ActiveStatus) {
        if(status.category != null) {
            this.actualDiv.innerHTML = `${status.year} | ${status.category}`;
        }
        else {
            this.actualDiv.innerHTML = status.year.toString();
        }
    }

    private updateStatus() {
        // TODO: anything we want to show here?  fps? help/keybindings? exit? about?
    }
}
