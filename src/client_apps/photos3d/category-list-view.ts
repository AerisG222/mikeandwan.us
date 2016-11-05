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
                private dataService: DataService) {

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
                let list = new List(categories);
                let years = list.GroupBy(cat => cat.year, cat => cat);
                
                for (var year in years) {
                    if (years.hasOwnProperty(year)) {
                        this.prepareCategoriesForYear(parseInt(year), years[year]);
                    }
                }

                /*
                for(let i = 0; i < years.length; i++) {
                    
                    let category = categories[i];
                    let year = this.getYear(category.year);
                    let categoryObject = this.createCategoryObject(category);
                    
                    this.scene.add(categoryObject);
                }
                */
            });
    }

    private prepareCategoriesForYear(year: number, categories: Array<ICategory>) {
        console.log(year);
    }

    private removeCategories() {
        if(this.years != null) {
            for(let i = 0; i < this.years.length; i++) {
                let year = this.years[i];

                for(let j = 0; j < year.categories.length; j++) {
                    this.scene.remove(year.categories[j]);
                }
            }
        }
    }

    private createCategoryObject(category: ICategory): CategoryObject3D {
        let x = Math.random() * this.width - (this.width * 0.5);
        let y = Math.random() * this.height;
        let z = Math.random() * 200;
        let endPos = new THREE.Vector3(x, y, z);
        let color = Math.floor(Math.random() * 0xffffff);

        let cat = new CategoryObject3D(category, endPos, color);
        cat.position.x = x;
        cat.position.y = y;
        cat.position.z = z;

        cat.init();

        return cat;
    }
    
    private getYear(year: number): IYear {
        let list = this.years.filter(iyear => { iyear.year == year });

        if(list.length === 1) {
            return list[0];
        }
        else if(list.length === 0) {
            let y = <IYear> { year: year, categories: [] };
            this.years.push(y);

            return y;
        }

        throw new Error('More than one entry found for the same year!');
    }
}
