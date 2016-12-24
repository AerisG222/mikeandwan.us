import { List } from 'linqts/linq';
import { Subscription } from 'rxjs/Subscription';

import { ArgumentNullError } from '../models/argument-null-error';
import { ArrowVisual } from '../visuals/arrow-visual';
import { Category } from '../models/category';
import { CategoryLayoutCalculator } from '../services/category-layout-calculator';
import { CategoryVisual } from '../visuals/category-visual';
import { DataService } from '../services/data-service';
import { DisposalService } from '../services/disposal-service';
import { FrustrumCalculator } from '../services/frustrum-calculator';
import { ICategory } from '../models/icategory';
import { IController } from './icontroller';
import { StateService } from '../services/state-service';
import { VisualContext } from '../models/visual-context';
import { YearVisual } from '../visuals/year-visual';

export class CategoryListController implements IController {
    private static readonly zWhenDisplayed = 500;

    private _ctx: VisualContext;
    private _categorySelectedSubscription: Subscription;

    private _disposed = false;
    private _lastElapsed = 0;
    private _idx = 0;
    private _visualsEnabled = true;
    private _yearList: Array<YearVisual> = [];
    private _heightWhenDisplayed: number;
    private _widthWhenDisplayed: number;
    private _prevArrow: ArrowVisual;
    private _prevArrowDisplayed = false;
    private _nextArrow: ArrowVisual;
    private _nextArrowDisplayed = false;

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

        if (_disposalService == null) {
            throw new ArgumentNullError('_disposalService');
        }

        this._ctx = _stateService.visualContext;

        let bounds = this._frustrumCalculator.calculateBounds(this._ctx.camera, CategoryListController.zWhenDisplayed);

        this._heightWhenDisplayed = bounds.y;
        this._widthWhenDisplayed = bounds.x;

        this._categorySelectedSubscription = this._stateService.categorySelectedObservable.subscribe(cat => this.onCategorySelected(cat));
    }

    get areVisualsEnabled(): boolean {
        return this._visualsEnabled;
    }

    init() {
        this.loadCategories();
    }

    moveNextYear() {
        if (this._disposed) {
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
        if (this._disposed) {
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

    render(delta: number, elapsed: number) {
        if (this._disposed) {
            return;
        }

        this._lastElapsed = elapsed;

        if (!this._visualsEnabled) {
            return;
        }

        if (this._nextArrow != null) {
            this._nextArrow.render(delta, elapsed);
        }

        if (this._prevArrow != null) {
            this._prevArrow.render(delta, elapsed);
        }

        for (let i = 0; i < this._yearList.length; i++) {
            let year = this._yearList[i];

            for (let j = 0; j < year.categoryList.length; j++) {
                year.categoryList[j].visual.render(delta, elapsed);
            }
        }
    }

    enableVisuals(areEnabled: boolean): void {
        this._visualsEnabled = areEnabled;

        if (!areEnabled) {
            this._yearList[this._idx].removeFromView();
        } else {
            this.updateYearsElapsedTime();
            this._yearList[this._idx].bringIntoView();
        }
    }

    dispose(): void {
        if (!this._disposed) {
            this._disposed = true;

            this._categorySelectedSubscription.unsubscribe();
            this._categorySelectedSubscription = null;

            this._disposalService.dispose(this._nextArrow);
            this._nextArrow = null;

            this._disposalService.dispose(this._prevArrow);
            this._prevArrow = null;

            for (let i = 0; i < this._yearList.length; i++) {
                this._yearList[i].dispose();
                this._yearList[i] = null;
            }

            this._yearList = null;
        }
    }

    private prepareArrows(): void {
        this._nextArrow = new ArrowVisual();
        this._nextArrow.init();
        this._nextArrow.position.set((this._widthWhenDisplayed / 2) - this._nextArrow.width,
                                     this._heightWhenDisplayed / 2,
                                     CategoryListController.zWhenDisplayed);
        this._nextArrow.clickObservable.subscribe(() => { this.moveNextYear(); });

        this._prevArrow = new ArrowVisual();
        this._prevArrow.init();
        this._prevArrow.rotateY(Math.PI);
        this._prevArrow.position.set(-(this._widthWhenDisplayed / 2) + this._prevArrow.width,
                                     this._heightWhenDisplayed / 2,
                                     CategoryListController.zWhenDisplayed);
        this._prevArrow.clickObservable.subscribe(() => { this.movePrevYear(); });

        this.updateArrowVisibility();
    }

    private updateArrowVisibility(): void {
        if (this._idx > 0 && !this._nextArrowDisplayed) {
            this._ctx.scene.add(this._nextArrow);
            this._nextArrowDisplayed = true;
        } else if (this._idx === 0 && this._nextArrowDisplayed) {
            this._ctx.scene.remove(this._nextArrow);
            this._nextArrowDisplayed = false;
        }

        if (this._idx < this._yearList.length - 1) {
            this._ctx.scene.add(this._prevArrow);
            this._prevArrowDisplayed = true;
        } else if (this._idx === this._yearList.length - 1 && this._prevArrowDisplayed) {
            this._ctx.scene.remove(this._prevArrow);
            this._prevArrowDisplayed = false;
        }
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
                this.prepareArrows();
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

        let clc = new CategoryLayoutCalculator(this._heightWhenDisplayed, this._widthWhenDisplayed);
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
                                                            new THREE.Vector3(lp.center.x,
                                                                              lp.center.y,
                                                                              CategoryListController.zWhenDisplayed),
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

    private getOffscreenPosition(): THREE.Vector3 {
        let min = -3000;
        let fullLength = 2 * Math.abs(min);
        let x = min + (Math.random() * fullLength);
        let y = min + (Math.random() * fullLength);
        let z = Math.random() * 2000 + (1000 + this._ctx.camera.position.z);

        return new THREE.Vector3(x, y, z);
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
