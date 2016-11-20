import { ICategory } from './icategory';
import { CategoryObject3D } from './category-object3d';
import { CategoryLayoutCalculator } from './category-layout-calculator';
import { CategoryLayout } from './category-layout';
import { StateService } from './state-service';

export class YearObject3D extends THREE.Object3D {
    private color: number;
    private categoryObject3dList: Array<CategoryObject3D> = [];

    constructor(public year: number,
                private stateService: StateService,
                private index: number,
                private heightWhenDisplayed: number,
                private widthWhenDisplayed: number,
                private zWhenDisplayed: number,
                private zBetweenYears: number,
                private categories: Array<ICategory>) {
        super();
        this.color = this.generateColor();
    }

    init() {
        this.position.z = this.zWhenDisplayed - (this.index * this.zBetweenYears);

        this.prepareCategories();
    }

    render(delta: number) {
        for(let i = 0; i < this.categoryObject3dList.length; i++) {
            this.categoryObject3dList[i].render(delta);
        }
    }

    private generateColor(): number {
        let r = Math.floor(Math.random() * 255);
        let g = Math.floor(Math.random() * 255);
        let b = Math.floor(Math.random() * 255);

        return parseInt(`${r.toString(16)}${g.toString(16)}${b.toString(16)}`, 16);
    }

    private prepareCategories() {
        let clc = new CategoryLayoutCalculator(this.heightWhenDisplayed, this.widthWhenDisplayed);
        let layout = clc.calculate(this.categories.length);

        for(let row = 0; row < layout.positions.length; row++) {
            for(let col = 0; col < layout.positions[row].length; col++) {
                let lp = layout.positions[row][col];

                if(lp.index < this.categories.length) {
                    let categoryObject = new CategoryObject3D(this.stateService,
                                                              this.categories[lp.index],
                                                              layout.hexagon,
                                                              this.getOffscreenPosition(),
                                                              lp.center, 
                                                              this.color);

                    categoryObject.init();

                    this.categoryObject3dList.push(categoryObject);
                    this.add(categoryObject);
                }
            }
        }
    }

    private removeCategories(): void {
        for(let i = 0; i < this.categoryObject3dList.length; i++) {
            this.remove(this.categoryObject3dList[i]);
        }
    }

    private getOffscreenPosition(): THREE.Vector3 {
        let min = -3000;
        let fullLength = 2 * Math.abs(min);
        let x = min + (Math.random() * fullLength);
        let y = min + (Math.random() * fullLength);
        let z = Math.random() * 2000 + (1000 + this.stateService.Camera.position.z);

        return new THREE.Vector3(x, y, z);        
    }

    private RandomBoolean
}
