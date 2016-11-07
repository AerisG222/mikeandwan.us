import { ICategory } from './icategory';
import { Year } from './year';
import { DataService } from './data-service';
import { List } from 'linqts/linq';

export class CategoryListView {
    private years: Array<Year> = [];

    constructor(private scene: THREE.Scene,
                private width: number,
                private height: number,
                private dataService: DataService,
                private zWhenDisplayed: number,
                private zBetweenYears: number) {

    }

    init() {
        this.loadCategories();
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

        for (let i = 0; i < yearKeys.length; i++) {
            let key = yearKeys[i];
            let year = new Year(parseInt(key), this.scene, i, this.zWhenDisplayed, this.zBetweenYears, categoryMap[key]);

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
