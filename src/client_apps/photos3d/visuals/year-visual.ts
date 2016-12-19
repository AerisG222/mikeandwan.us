import { ArgumentNullError } from '../models/argument-null-error';
import { Category } from '../models/category';
import { DisposalService } from '../services/disposal-service';
import { IVisual } from './ivisual';

export class YearVisual extends THREE.Object3D implements IVisual {
    private _disposed = false;

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

    render(): void {

    }

    dispose(): void {
        if (!this._disposed) {
            this._disposed = true;

            for (let i = 0; i < this._categoryList.length; i++) {
                this._categoryList[i].visual.dispose();
                this._categoryList[i] = null;
            }

            this._disposalService.dispose(this);
            this._categoryList = null;
        }
    }
}
