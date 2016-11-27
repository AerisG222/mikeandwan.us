import { ArgumentNullError } from '../models/argument-null-error';
import { Subject } from 'rxjs/Subject';
import { ActiveStatus } from '../models/active-status';
import { VisualContext } from '../models/visual-context';

export class StateService {
    private _activeNavSubject = new Subject<ActiveStatus>();
    private _temporalNavSubject = new Subject<string>();
    private _mouseoverSubject = new Subject<Array<THREE.Intersection>>();
    private _visualContext: VisualContext;

    constructor(context: VisualContext) {
        if (context == null) {
            throw new ArgumentNullError('context');
        }

        this._visualContext = context;
    }

    get visualContext() {
        return this._visualContext;
    }

    get activeNavObservable() {
        return this._activeNavSubject.asObservable();
    }

    get temporalNavObservable() {
        return this._temporalNavSubject.asObservable();
    }

    get mouseoverObservable() {
        return this._mouseoverSubject.asObservable();
    }

    updateTemporalNav(category: string) {
        this._temporalNavSubject.next(category);
    }

    updateActiveNav(year: number, category?: string) {
        this._activeNavSubject.next(new ActiveStatus(year, category));
    }

    updateMouseover(intersections: Array<THREE.Intersection>) {
        this.updateTemporalNav(null);
        this._mouseoverSubject.next(intersections);
    }
}
