import { Subject } from 'rxjs/Subject';

import { ActiveStatus } from '../models/active-status';
import { ArgumentNullError } from '../models/argument-null-error';
import { ICategory } from '../models/icategory';
import { VisualContext } from '../models/visual-context';

export class StateService {
    private _activeNavSubject = new Subject<ActiveStatus>();
    private _temporalNavSubject = new Subject<string>();
    private _mouseoverSubject = new Subject<Array<THREE.Intersection>>();
    private _mouseclickSubject = new Subject<Array<THREE.Intersection>>();
    private _categorySelectedSubject = new Subject<ICategory>();
    private _dialogDisplayedSubject = new Subject<boolean>();
    private _pauseSubject = new Subject<boolean>();
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

    get mouseclickObservable() {
        return this._mouseclickSubject.asObservable();
    }

    get categorySelectedObservable() {
        return this._categorySelectedSubject.asObservable();
    }

    get dialogDisplayedObservable() {
        return this._dialogDisplayedSubject.asObservable();
    }

    get pausedObservable() {
        return this._pauseSubject.asObservable();
    }

    publishTemporalNav(category: string) {
        this._temporalNavSubject.next(category);
    }

    publishActiveNav(year: number, category?: string) {
        this._activeNavSubject.next(new ActiveStatus(year, category));
    }

    publishMouseover(intersections: Array<THREE.Intersection>) {
        this.publishTemporalNav(null);
        this._mouseoverSubject.next(intersections);
    }

    publishMouseClick(intersections: Array<THREE.Intersection>) {
        this._mouseclickSubject.next(intersections);
    }

    publishCategorySelected(category: ICategory) {
        this._categorySelectedSubject.next(category);
    }

    publishDialogDisplayed(isDisplayed: boolean) {
        this._dialogDisplayedSubject.next(isDisplayed);
    }

    publishPaused(isPaused: boolean) {
        this._pauseSubject.next(isPaused);
    }
}
