import { List } from 'linqts/linq';

import { ArgumentNullError } from '../models/argument-null-error';
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
import { Year } from '../models/year';

export class CategoryListController implements IController {
    private static readonly zWhenDisplayed = 500;

    private _ctx: VisualContext;

    private _idx = 0;
    private _visualsEnabled = true;
    private _yearList: Array<Year> = [];
    private heightWhenDisplayed: number;
    private widthWhenDisplayed: number;

    constructor(private dataService: DataService,
                private stateService: StateService,
                private frustrumCalculator: FrustrumCalculator,
                private _disposalService: DisposalService) {
        if (dataService == null) {
            throw new ArgumentNullError('dataService');
        }

        if (stateService == null) {
            throw new ArgumentNullError('stateService');
        }

        if (_disposalService == null) {
            throw new ArgumentNullError('_disposalService');
        }

        this._ctx = stateService.visualContext;

        let bounds = this.frustrumCalculator.calculateBounds(this._ctx.camera, CategoryListController.zWhenDisplayed);

        this.heightWhenDisplayed = bounds.y;
        this.widthWhenDisplayed = bounds.x;

        this.stateService.categorySelectedObservable.subscribe(cat => this.onCategorySelected(cat));
    }

    get areVisualsEnabled(): boolean {
        return this._visualsEnabled;
    }

    init() {
        this.loadCategories();
    }

    moveNextYear() {
        if (this._idx > 0) {
            this._yearList[this._idx].removeFromView();
            this._idx--;
            this._yearList[this._idx].bringIntoView();

            this.stateService.publishActiveNav(this._yearList[this._idx].year);
        }
    }

    movePrevYear() {
        if (this._idx < this._yearList.length - 1) {
            this._yearList[this._idx].removeFromView();
            this._idx++;
            this._yearList[this._idx].bringIntoView();

            this.stateService.publishActiveNav(this._yearList[this._idx].year);
        }
    }

    render(delta: number, elapsed: number) {
        if (!this._visualsEnabled) {
            return;
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
            this._yearList[this._idx].bringIntoView();
        }
    }

    private onCategorySelected(category: ICategory) {
        this.enableVisuals(false);
    }

    private loadCategories() {
        this.dataService
            .getCategories()
            .then(categories => {
                this.prepareAllYears(categories);

                this.stateService.publishActiveNav(this._yearList[0].year);
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
            this._ctx.scene.add(year.container);

            if (i === 0) {
                year.bringIntoView();
            }
        }
    }

    private prepareYear(yearIndex: number, theYear: number, categories: Array<ICategory>): Year {
        let year = new Year(theYear, this.generateColor());

        let clc = new CategoryLayoutCalculator(this.heightWhenDisplayed, this.widthWhenDisplayed);
        let layout = clc.calculate(categories.length);
        let catIndex = 0;

        for (let row = 0; row < layout.positions.length; row++) {
            for (let col = 0; col < layout.positions[row].length; col++) {
                let lp = layout.positions[row][col];
                let cat = categories[catIndex];

                if (lp.index < categories.length) {
                    let categoryVisual = new CategoryVisual(this.stateService,
                                                            cat,
                                                            layout.hexagon,
                                                            new THREE.Vector3(lp.center.x,
                                                                              lp.center.y,
                                                                              CategoryListController.zWhenDisplayed),
                                                            this.getOffscreenPosition(),
                                                            year.color);

                    categoryVisual.init();

                    year.categoryList.push(new Category(cat, categoryVisual));
                    year.container.add(categoryVisual);
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
