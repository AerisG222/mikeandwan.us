import { Component, OnInit } from '@angular/core';

import { VideoDataService } from '../shared/video-data.service';
import { VideoNavigationService } from '../shared/video-navigation.service';

@Component({
    moduleId: module.id,
    selector: 'app-year-list',
    templateUrl: 'year-list.component.html',
    styleUrls: [ 'year-list.component.css' ]
})
export class YearListComponent implements OnInit {
    year: number = -1;
    years: Array<number> = [];

    constructor(private _dataService: VideoDataService,
        private _navService: VideoNavigationService) {

    }

    selectItem(year: number): void {
        this._navService.gotoCategoryList(year);
    };

    ngOnInit(): void {
        this._dataService.getYears()
            .subscribe(
            (data: Array<number>) => this.years = data,
            (err: any) => console.error(`there was an error: ${err}`)
            );
    }
}
