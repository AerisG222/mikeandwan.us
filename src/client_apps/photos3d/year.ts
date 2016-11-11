import { ICategory } from './icategory';
import { CategoryObject3D } from './category-object3d';
import { CategoryLayoutCalculator } from './category-layout-calculator';
import { CategoryLayout } from './category-layout';

export class Year {
    private color: number;
    private z: number;
    private categoryObject3dList: Array<CategoryObject3D> = [];

    constructor(public year: number,
                private scene: THREE.Scene,
                private index: number,
                private heightWhenDisplayed: number,
                private widthWhenDisplayed: number,
                private zWhenDisplayed: number,
                private zBetweenYears: number,
                private categories: Array<ICategory>) {
        this.color = this.generateColor();
    }

    init() {
        this.z = this.zWhenDisplayed - (this.index * this.zBetweenYears);
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
        let z = this.zWhenDisplayed - (this.zBetweenYears * this.index);
        let clc = new CategoryLayoutCalculator();
        let layout = clc.calculate(this.categories.length);

        for(let i = 0; i < this.categories.length; i++) {
            let category = this.categories[i];
            let pos = this.getCategoryPosition(i, this.categories.length);
            let categoryObject = new CategoryObject3D(category, pos, this.color); 
            
            categoryObject.init();

            this.scene.add(categoryObject);
        }
    }

    private removeCategories() {
        for(let i = 0; i < this.categoryObject3dList.length; i++) {
            this.scene.remove(this.categoryObject3dList[i]);
        }
    }

    private getCategoryPosition(index: number, count: number) {
        let x = Math.random() * this.widthWhenDisplayed - (this.widthWhenDisplayed * 0.5);
        let y = Math.random() * this.heightWhenDisplayed;
        let endPos = new THREE.Vector3(x, y, this.z);

        return endPos;
    }
}
