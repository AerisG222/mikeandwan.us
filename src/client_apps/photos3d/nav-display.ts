import { StateService } from './state-service';
import { NavEvent } from './nav-event';
import { NavEventType } from './nav-event-type';

export class NavDisplay {
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
        this.temporalDiv.style.width = '33%';
        this.temporalDiv.innerHTML = 'temp';
        this.navDiv.appendChild(this.temporalDiv);

        this.statusDiv = document.createElement('div');
        this.statusDiv.style.width = '33%';
        this.statusDiv.innerHTML = 'stat';
        this.navDiv.appendChild(this.statusDiv);
    }

    private createInfoDiv(width: string, align: string) {
        let div = document.createElement('div');

        div.style.width = width;
        div.style.cssFloat = 'left';

        return div;
    }

    private updateTemporal(event: NavEvent) {
        this.temporalDiv.innerHTML = event.navItem.name;
    }

    private updateActive(event: NavEvent) {
        if(event.navEventType == NavEventType.Remove) {

        }
        else if(event.navEventType == NavEventType.Add) {
            this.actualDiv.innerHTML += ` | ${event.navItem.name}`;
        }
        else if(event.navEventType == NavEventType.Update) {
            this.actualDiv.innerHTML = event.navItem.name;
        }
    }

    private updateStatus() {
        // TODO: anything we want to show here?  fps? help/keybindings? exit? about?
    }
}
