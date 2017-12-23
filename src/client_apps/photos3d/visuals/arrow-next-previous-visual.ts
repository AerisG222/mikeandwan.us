import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';

import { Group } from 'three';

import { ArgumentNullError } from '../models/argument-null-error';
import { ArrowVisual } from './arrow-visual';
import { DisposalService } from '../services/disposal-service';
import { FrustrumCalculator } from '../services/frustrum-calculator';
import { IDisposable } from '../models/idisposable';
import { IVisual } from './ivisual';
import { VisualContext } from '../models/visual-context';

export class ArrowNextPreviousVisual extends Group implements IDisposable, IVisual {
    private _isDisposed = false;

    private _boundsHeight: number;
    private _boundsWidth: number;
    private _boundsDepth: number;

    private _nextArrow: ArrowVisual;
    private _nextArrowDisplayed = false;
    private _nextSubject = new Subject<null>();

    private _prevArrow: ArrowVisual;
    private _prevArrowDisplayed = false;
    private _prevSubject = new Subject<null>();

    constructor(private _visualContext: VisualContext,
                private _frustrumCalculator: FrustrumCalculator,
                private _disposalService: DisposalService) {
        super();

        if (_visualContext == null) {
            throw new ArgumentNullError('_visualContext');
        }

        if (_frustrumCalculator == null) {
            throw new ArgumentNullError('_frustrumCalculator');
        }

        if (_disposalService == null) {
            throw new ArgumentNullError('_disposalService');
        }

        this._boundsDepth = this._frustrumCalculator.calculateZForFullFrame(this._visualContext.camera);

        let bounds = this._frustrumCalculator.calculateBounds(this._visualContext.camera, this._boundsDepth);

        this._boundsHeight = bounds.y;
        this._boundsWidth = bounds.x;
    }

    get nextObservable(): Observable<null> {
        return this._nextSubject.asObservable();
    }

    get prevObservable(): Observable<null> {
        return this._prevSubject.asObservable();
    }

    init(): void {
        this.prepareArrows();
    }

    render(clockDelta: number, elapsed: number): void {
        if (this._isDisposed) {
            return;
        }

        if (this._nextArrow != null) {
            this._nextArrow.render(clockDelta, elapsed);
        }

        if (this._prevArrow != null) {
            this._prevArrow.render(clockDelta, elapsed);
        }
    }

    dispose(): void {
        if (this._isDisposed) {
            return;
        }

        this.remove(this._nextArrow);
        this._disposalService.dispose(this._nextArrow);
        this._nextArrow = null;

        this.remove(this._prevArrow);
        this._disposalService.dispose(this._prevArrow);
        this._prevArrow = null;
    }

    showNext(doShow: boolean): void {
        if (doShow === this._nextArrowDisplayed) {
            return;
        }

        this._nextArrowDisplayed = doShow;

        if (doShow) {
            this.add(this._nextArrow);
        } else {
            this.remove(this._nextArrow);
        }
    }

    showPrevious(doShow: boolean): void {
        if (doShow === this._prevArrowDisplayed) {
            return;
        }

        this._prevArrowDisplayed = doShow;

        if (doShow) {
            this.add(this._prevArrow);
        } else {
            this.remove(this._prevArrow);
        }
    }

    private prepareArrows(): void {
        this._nextArrow = new ArrowVisual(this._disposalService);
        this._nextArrow.init();
        this._nextArrow.position.set((this._boundsWidth / 2) - this._nextArrow.width,
                                     this._boundsHeight / 2,
                                     this._boundsDepth);
        this._nextArrow.clickObservable.subscribe(() => { this.onNextClicked(); });

        this._prevArrow = new ArrowVisual(this._disposalService);
        this._prevArrow.init();
        this._prevArrow.rotateY(Math.PI);
        this._prevArrow.position.set(-(this._boundsWidth / 2) + this._prevArrow.width,
                                     this._boundsHeight / 2,
                                     this._boundsDepth);
        this._prevArrow.clickObservable.subscribe(() => { this.onPreviousClicked(); });
    }

    private onNextClicked(): void {
        this._nextSubject.next();
    }

    private onPreviousClicked(): void {
        this._prevSubject.next();
    }
}
