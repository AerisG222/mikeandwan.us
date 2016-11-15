import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';
import { ActiveStatus } from './active-status';

export class StateService {
    private activeNavSubject = new Subject<ActiveStatus>();
    private temporalNavSubject = new Subject<string>();

    get ActiveNavObservable() {
        return this.activeNavSubject.asObservable();
    }

    get TemporalNavObservable() {
        return this.temporalNavSubject.asObservable();
    }

    updateTemporalNav(category: string) {
        this.temporalNavSubject.next(category);
    }

    updateActiveNav(year: number, category?: string) {
        this.activeNavSubject.next(new ActiveStatus(year, category));
    }
}
