import * as THREE from 'three';

import { ArgumentNullError } from '../models/argument-null-error';
import { Category } from '../models/category';
import { DisposalService } from '../services/disposal-service';
import { IDisposable } from '../models/idisposable';
import { IVisual } from './ivisual';

export class YearVisual extends THREE.Object3D implements IDisposable, IVisual {
    private _isDisposed = false;

    private _categoryList: Array<Category>;

    constructor(private _disposalService: DisposalService,
                public year: number,
                public color: number,
                categoryList?: Array<Category>) {
        super();

        if (_disposalService == null) {
            throw new ArgumentNullError('_disposalService');
        }

        if (year == null) {
            throw new ArgumentNullError('year');
        }

        if (categoryList == null) {
            this._categoryList = [];
        } else {
            this._categoryList = categoryList;
        }
    }

    get categoryList(): Array<Category> {
        return this._categoryList;
    }

    init() {

    }

    bringIntoView(): void {
        for (let i = 0; i < this._categoryList.length; i++) {
            this._categoryList[i].visual.bringIntoView();
        }
    }

    removeFromView(): void {
        for (let i = 0; i < this._categoryList.length; i++) {
            this._categoryList[i].visual.removeFromView();
        }
    }

    updateElapsedTime(elapsed: number): void {
        for (let i = 0; i < this._categoryList.length; i++) {
            this._categoryList[i].visual.updateElapsedTime(elapsed);
        }
    }

    render(clockDelta: number, elapsed: number): void {
        for (let i = 0; i < this._categoryList.length; i++) {
            this._categoryList[i].visual.render(clockDelta, elapsed);
        }
    }

    dispose(): void {
        if (this._isDisposed) {
            return;
        }

        this._isDisposed = true;

        for (let i = 0; i < this._categoryList.length; i++) {
            this.remove(this._categoryList[i].visual);
            this._disposalService.dispose(this._categoryList[i].visual);
            this._categoryList[i] = null;
        }

        this._categoryList = null;
    }
}
