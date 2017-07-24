import { Component, OnInit } from '@angular/core';
import { trigger, state, style, animate, transition } from '@angular/animations';

import { VideoDataService } from '../shared/video-data.service';
import { VideoNavigationService } from '../shared/video-navigation.service';

@Component({
    selector: 'app-year-list',
    templateUrl: './year-list.component.html',
    styleUrls: [ './year-list.component.css' ],
    animations: [
        trigger('fadeInOut', [
            state('in', style({opacity: 1})),
            transition('void => *', [
                style({opacity: 0}),
                animate(320)
            ]),
            transition('* => void', [
                animate(320, style({opacity: 1}))
            ])
        ])
    ]
})
export class YearListComponent implements OnInit {
    year: number = -1;
    years: Array<number> = [];

    constructor(private _dataService: VideoDataService,
                private _navService: VideoNavigationService) {

    }

    selectItem(year: number): void {
        this._navService.gotoCategoryList(year);
    }

    ngOnInit(): void {
        this._dataService.getYears()
            .subscribe(
            (data: Array<number>) => this.years = data,
            (err: any) => console.error(`there was an error: ${err}`)
            );
    }
}
