import { ICategory } from './icategory';
import { CategoryObject3D } from './category-object3d';

export class Year {
    private color: number;
    private width = 800;
    private height = 500;
    private z: number;
    private categoryObject3dList: Array<CategoryObject3D> = [];

    constructor(public year: number,
                private scene: THREE.Scene,
                private index: number,
                private zWhenDisplayed: number,
                private zBetweenYears: number,
                private categories: Array<ICategory>) {
        this.color = this.generateColor();
    }

    public init() {
        this.z = this.zWhenDisplayed - (this.index * this.zBetweenYears);
        this.prepareCategories();
    }

    private generateColor(): number {
        let r = Math.floor(Math.random() * 255);
        let g = Math.floor(Math.random() * 255);
        let b = Math.floor(Math.random() * 255);

        return parseInt(`${r.toString(16)}${g.toString(16)}${b.toString(16)}`, 16);
    }

    private prepareCategories() {
        let z = this.zWhenDisplayed - (this.zBetweenYears * this.index);

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
        let x = Math.random() * this.width - (this.width * 0.5);
        let y = Math.random() * this.height;
        let endPos = new THREE.Vector3(x, y, this.z);

        return endPos;
    }
}
