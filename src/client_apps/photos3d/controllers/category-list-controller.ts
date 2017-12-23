import { List } from 'linqts';
import { Subscription } from 'rxjs/Subscription';

import { Vector3 } from 'three';

import { ArgumentNullError } from '../models/argument-null-error';
import { ArrowNextPreviousVisual } from '../visuals/arrow-next-previous-visual';
import { Category } from '../models/category';
import { CategoryLayoutCalculator } from '../services/category-layout-calculator';
import { CategoryVisual } from '../visuals/category-visual';
import { DataService } from '../services/data-service';
import { DisposalService } from '../services/disposal-service';
import { FrustrumCalculator } from '../services/frustrum-calculator';
import { ICategory } from '../models/icategory';
import { IController } from './icontroller';
import { IDisposable } from '../models/idisposable';
import { StateService } from '../services/state-service';
import { VisualContext } from '../models/visual-context';
import { YearVisual } from '../visuals/year-visual';

export class CategoryListController implements IController, IDisposable {
    private _arrows: ArrowNextPreviousVisual;
    private _categorySelectedSubscription: Subscription;
    private _ctx: VisualContext;
    private _idx = 0;
    private _isDisposed = false;
    private _lastElapsed = 0;
    private _maxDepth: number;
    private _maxHeight: number;
    private _maxWidth: number;
    private _visualsEnabled = true;
    private _yearList: Array<YearVisual> = [];

    constructor(private _dataService: DataService,
                private _stateService: StateService,
                private _frustrumCalculator: FrustrumCalculator,
                private _disposalService: DisposalService) {
        if (_dataService == null) {
            throw new ArgumentNullError('_dataService');
        }

        if (_stateService == null) {
            throw new ArgumentNullError('_stateService');
        }

        if (_frustrumCalculator == null) {
            throw new ArgumentNullError('_frustrumCalculator');
        }

        if (_disposalService == null) {
            throw new ArgumentNullError('_disposalService');
        }

        this._ctx = _stateService.visualContext;
        this._maxDepth = this._frustrumCalculator.calculateZForFullFrame(this._ctx.camera) - 20;

        let bounds = this._frustrumCalculator.calculateBounds(this._ctx.camera, this._maxDepth);

        this._maxHeight = bounds.y;
        this._maxWidth = bounds.x;

        this._categorySelectedSubscription = this._stateService.categorySelectedObservable.subscribe(cat => this.onCategorySelected(cat));

        this._arrows = new ArrowNextPreviousVisual(this._ctx, this._frustrumCalculator, this._disposalService);
        this._arrows.init();
        this._arrows.nextObservable.subscribe(() => this.moveNextYear());
        this._arrows.prevObservable.subscribe(() => this.movePrevYear());
        this._ctx.scene.add(this._arrows);
    }

    get areVisualsEnabled(): boolean {
        return this._visualsEnabled;
    }

    init() {
        this.loadCategories();
    }

    moveNextYear() {
        if (this._isDisposed) {
            return;
        }

        if (this._idx > 0) {
            this._yearList[this._idx].removeFromView();
            this._idx--;
            this._yearList[this._idx].bringIntoView();

            this._stateService.publishActiveNav(this._yearList[this._idx].year);

            this.updateArrowVisibility();
        }
    }

    movePrevYear() {
        if (this._isDisposed) {
            return;
        }

        if (this._idx < this._yearList.length - 1) {
            this._yearList[this._idx].removeFromView();
            this._idx++;
            this._yearList[this._idx].bringIntoView();

            this._stateService.publishActiveNav(this._yearList[this._idx].year);

            this.updateArrowVisibility();
        }
    }

    render(clockDelta: number, elapsed: number) {
        if (this._isDisposed) {
            return;
        }

        this._lastElapsed = elapsed;

        if (!this._visualsEnabled) {
            return;
        }

        this._arrows.render(clockDelta, elapsed);

        for (let i = 0; i < this._yearList.length; i++) {
            this._yearList[i].render(clockDelta, elapsed);
        }
    }

    enableVisuals(areEnabled: boolean): void {
        this._visualsEnabled = areEnabled;

        if (!areEnabled) {
            this._yearList[this._idx].removeFromView();
            this._ctx.scene.remove(this._arrows);
        } else {
            this.updateYearsElapsedTime();
            this._yearList[this._idx].bringIntoView();
            this._ctx.scene.add(this._arrows);
        }
    }

    dispose(): void {
        if (this._isDisposed) {
            return;
        }

        this._isDisposed = true;

        this._categorySelectedSubscription.unsubscribe();
        this._categorySelectedSubscription = null;

        this._ctx.scene.remove(this._arrows);
        this._disposalService.dispose(this._arrows);
        this._arrows = null;

        for (let i = 0; i < this._yearList.length; i++) {
            this._ctx.scene.remove(this._yearList[i]);
            this._disposalService.dispose(this._yearList[i]);
            this._yearList[i] = null;
        }

        this._yearList = null;
    }

    private updateArrowVisibility(): void {
        this._arrows.showNext(this._idx > 0);
        this._arrows.showPrevious(this._idx < this._yearList.length - 1);
    }

    private updateYearsElapsedTime(): void {
        for (let i = 0; i < this._yearList.length; i++) {
            this._yearList[i].updateElapsedTime(this._lastElapsed);
        }
    }

    private onCategorySelected(category: ICategory) {
        this.enableVisuals(false);
    }

    private loadCategories() {
        this._dataService
            .getCategories()
            .then(categories => {
                this.prepareAllYears(categories);
                this.updateArrowVisibility();
                this._stateService.publishActiveNav(this._yearList[0].year);
            });
    }

    private prepareAllYears(categories: Array<ICategory>) {
        let list = new List(categories);
        let categoryMap = list.GroupBy(cat => cat.year, cat => cat);
        let yearKeys = this.getSortedYears(categoryMap);

        for (let i = 0; i < yearKeys.length; i++) {
            let key = yearKeys[i];

            let year = this.prepareYear(i, parseInt(key, 10), categoryMap[key]);

            this._yearList.push(year);
            this._ctx.scene.add(year);

            if (i === 0) {
                year.bringIntoView();
            }
        }
    }

    private prepareYear(yearIndex: number, theYear: number, categories: Array<ICategory>): YearVisual {
        let year = new YearVisual(this._disposalService, theYear, this.generateColor());

        let clc = new CategoryLayoutCalculator(this._maxHeight, this._maxWidth);
        let layout = clc.calculate(categories.length);
        let catIndex = 0;

        for (let row = 0; row < layout.positions.length; row++) {
            for (let col = 0; col < layout.positions[row].length; col++) {
                let lp = layout.positions[row][col];
                let cat = categories[catIndex];

                if (lp.index < categories.length) {
                    let categoryVisual = new CategoryVisual(this._stateService,
                                                            this._disposalService,
                                                            cat,
                                                            layout.hexagon,
                                                            new Vector3(lp.center.x,
                                                                              lp.center.y,
                                                                              this._maxDepth),
                                                            this.getOffscreenPosition(),
                                                            year.color);

                    categoryVisual.init();

                    year.categoryList.push(new Category(cat, categoryVisual));
                    year.add(categoryVisual);
                }

                catIndex++;
            }
        }

        return year;
    }

    private getOffscreenPosition(): Vector3 {
        let min = -3000;
        let fullLength = 2 * Math.abs(min);
        let x = min + (Math.random() * fullLength);
        let y = min + (Math.random() * fullLength);
        let z = Math.random() * 2000 + (1000 + this._ctx.camera.position.z);

        return new Vector3(x, y, z);
    }

    private generateColor(): number {
        let r = Math.floor(Math.random() * 255);
        let g = Math.floor(Math.random() * 255);
        let b = Math.floor(Math.random() * 255);

        return parseInt(`${r.toString(16)}${g.toString(16)}${b.toString(16)}`, 16);
    }

    private getSortedYears(categoryMap: any): Array<string> {
        let yearKeys: Array<string> = [];

        for (let yearKey in categoryMap) {
            if (categoryMap.hasOwnProperty(yearKey)) {
                yearKeys.push(yearKey);
            }
        }

        let yearList = new List(yearKeys);

        return yearList
            .OrderBy(key => key)
            .Reverse()
            .ToArray();
    }
}
