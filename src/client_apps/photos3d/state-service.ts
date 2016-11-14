import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';
import { NavEvent } from './nav-event';

export class StateService {
    private activeNavSubject = new Subject<NavEvent>();
    private temporalNavSubject = new Subject<NavEvent>();

    get ActiveNavObservable() {
        return this.activeNavSubject.asObservable();
    }

    get TemporalNavObservable() {
        return this.temporalNavSubject.asObservable();
    }

    updateTemporalNav(event: NavEvent) {
        this.temporalNavSubject.next(event);
    }

    updateActiveNav(event: NavEvent) {
        this.activeNavSubject.next(event);
    }
}
