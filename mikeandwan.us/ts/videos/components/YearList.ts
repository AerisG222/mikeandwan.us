import { Component, OnInit } from '@angular/core';
import { VideoDataService } from '../services/VideoDataService';
import { VideoNavigationService } from '../services/VideoNavigationService';

@Component({
    selector: 'yearlist',	
    directives: [  ],
    templateUrl: '/js/videos/components/YearList.html'
})
export class YearList implements OnInit {
    year : number = -1;
	years : Array<number> = [];
    
    constructor(private _dataService : VideoDataService, 
                private _navService : VideoNavigationService) {

    }
    
    selectItem(year : number) : void {
        this._navService.gotoCategoryList(year);
    };
    
    ngOnInit() : void {
        this._dataService.getYears()
            .subscribe(
                (data : Array<number>) => this.years = data,
                (err : any) => console.error(`there was an error: ${err}`)
            );
    }
}
