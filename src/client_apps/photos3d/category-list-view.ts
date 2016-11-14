import { ICategory } from './icategory';
import { YearObject3D } from './year-object3d';
import { DataService } from './data-service';
import { StateService } from './state-service';
import { List } from 'linqts/linq';
import { NavEvent } from './nav-event';
import { NavEventType } from './nav-event-type';
import { NavItem } from './nav-item';
import { NavType } from './nav-type';

export class CategoryListView {
    private years: Array<YearObject3D> = [];

    constructor(private scene: THREE.Scene,
                private camera: THREE.PerspectiveCamera,
                private width: number,
                private height: number,
                private dataService: DataService,
                private stateService: StateService,
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
            let year = new YearObject3D(parseInt(key), i, heightWhenDisplayed, widthWhenDisplayed, this.zWhenDisplayed, this.zBetweenYears, categoryMap[key]);

            year.init();

            this.scene.add(year);

            this.years.push(year);
        }

        // TODO: put this in a better spot
        this.stateService.updateActiveNav(new NavEvent(NavEventType.Update, new NavItem(NavType.Year, this.years[0].year.toString())));
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
