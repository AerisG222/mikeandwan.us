import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';
import { ActiveStatus } from './active-status';

export class StateService {
    private activeNavSubject = new Subject<ActiveStatus>();
    private temporalNavSubject = new Subject<string>();
    private mouseoverSubject = new Subject<Array<THREE.Intersection>>();

    get ActiveNavObservable() {
        return this.activeNavSubject.asObservable();
    }

    get TemporalNavObservable() {
        return this.temporalNavSubject.asObservable();
    }

    get MouseoverObservable() {
        return this.mouseoverSubject.asObservable();
    }

    updateTemporalNav(category: string) {
        this.temporalNavSubject.next(category);
    }

    updateActiveNav(year: number, category?: string) {
        this.activeNavSubject.next(new ActiveStatus(year, category));
    }

    updateMouseover(intersections: Array<THREE.Intersection>) {
        this.updateTemporalNav(null);
        this.mouseoverSubject.next(intersections);
    }
}
