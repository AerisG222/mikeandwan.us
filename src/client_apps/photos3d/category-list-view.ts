import { ICategory } from './icategory';
import { Year } from './year';
import { DataService } from './data-service';
import { List } from 'linqts/linq';

export class CategoryListView {
    private years: Array<Year> = [];

    constructor(private scene: THREE.Scene,
                private camera: THREE.PerspectiveCamera,
                private width: number,
                private height: number,
                private dataService: DataService,
                private zWhenDisplayed: number,
                private zBetweenYears: number) {
         
    }

    init() {
        this.loadCategories();
    }

    render(delta: number) {
        for(let i = 0; i < this.years.length; i++) {
            this.years[i].render(delta);
        }
    }

    private loadCategories() {
        this.dataService
            .getCategories()
            .then(categories => {
                this.prepareAllCategories(categories);
            });
    }

    private prepareAllCategories(categories: Array<ICategory>) {
        let list = new List(categories);
        let categoryMap = list.GroupBy(cat => cat.year, cat => cat);
        let yearKeys = this.getSortedYears(categoryMap);

        // http://gamedev.stackexchange.com/questions/96317/determining-view-boundaries-based-on-z-position-when-using-a-perspective-project
        let heightWhenDisplayed = 2 * Math.tan(this.camera.fov * 0.5 * (Math.PI/180)) * (this.camera.position.z - this.zWhenDisplayed);
        let widthWhenDisplayed = heightWhenDisplayed * this.camera.aspect;

        for (let i = 0; i < yearKeys.length; i++) {
            let key = yearKeys[i];
            let year = new Year(parseInt(key), this.scene, i, heightWhenDisplayed, widthWhenDisplayed, this.zWhenDisplayed, this.zBetweenYears, categoryMap[key]);

            year.init();

            this.years.push(year);
        }
    }

    private getSortedYears(categoryMap: any) : Array<string> {
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
