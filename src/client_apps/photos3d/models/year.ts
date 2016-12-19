import { ArgumentNullError } from './argument-null-error';
import { Category } from './category';

export class Year {
    private _categoryList: Array<Category>;
    private _containerVisual = new THREE.Object3D();

    constructor(public year: number,
                public color: number,
                categoryList?: Array<Category>) {
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

    get container(): THREE.Object3D {
        return this._containerVisual;
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
}
