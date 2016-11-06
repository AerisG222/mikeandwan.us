import { CategoryObject3D } from './category-object3d';
import { ICategory } from './icategory';
import { IYear } from './iyear';
import { DataService } from './data-service';
import { List } from 'linqts/linq';

export class CategoryListView {
    private years: Array<IYear> = null;

    constructor(private scene: THREE.Scene,
                private width: number,
                private height: number,
                private dataService: DataService,
                private zDisplayed: number,
                private zSeparation: number) {

    }

    init() {
        this.loadCategories();
    }

    show() {

    }

    hide() {

    }

    private loadCategories() {
        this.dataService
            .getCategories()
            .then(categories => {
                this.prepareAllCategories(categories);
            });
    }

    private generateColor(): number {
        let r = Math.floor(Math.random() * 255);
        let g = Math.floor(Math.random() * 255);
        let b = Math.floor(Math.random() * 255);

        return parseInt(`${r.toString(16)}${g.toString(16)}${b.toString(16)}`, 16);
    }

    private prepareAllCategories(categories: Array<ICategory>) {
        let list = new List(categories);
        let years = list.GroupBy(cat => cat.year, cat => cat);
        let iyears: Array<IYear> = [];

        for (let year in years) {
            if (years.hasOwnProperty(year)) {
                let iyear = <IYear> {
                    categories: years[year],
                    categoryObjects: [],
                    color: this.generateColor()
                };
                
                iyears.push(iyear);
            }
        }

        let iyearList = new List(iyears);
        iyears = iyearList
            .OrderBy(key => key.year)
            .Reverse()
            .ToArray();

        for (let i = 0; i < iyears.length; i++) {
            let theYear = iyears[i];
            theYear.index = i;
            this.prepareCategoriesForYear(theYear);
        }
    }

    private prepareCategoriesForYear(year: IYear) {
        let z = this.zDisplayed - (this.zSeparation * year.index);

        for(let i = 0; i < year.categories.length; i++) {
            let category = year.categories[i];
            let categoryObject = this.createCategoryObject(category, z, year.color);
            
            this.scene.add(categoryObject);
        }
    }

    private removeCategories() {
        if(this.years != null) {
            for(let i = 0; i < this.years.length; i++) {
                let year = this.years[i];

                for(let j = 0; j < year.categoryObjects.length; j++) {
                    this.scene.remove(year.categoryObjects[j]);
                }
            }
        }
    }

    private createCategoryObject(category: ICategory, z: number, color: number): CategoryObject3D {
        let x = Math.random() * this.width - (this.width * 0.5);
        let y = Math.random() * this.height;
        let endPos = new THREE.Vector3(x, y, z);

        let cat = new CategoryObject3D(category, endPos, color);
        cat.position.x = x;
        cat.position.y = y;
        cat.position.z = z;

        cat.init();

        return cat;
    }
}
