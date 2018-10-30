import { Component, OnInit } from '@angular/core';
import { state, style, transition, trigger, useAnimation } from '@angular/animations';

import { fadeIn, fadeOut } from 'maw-common';
import { VideoDataService } from '../services/video-data.service';
import { Observable } from 'rxjs';
import { VideoNavigationService } from '../services/video-navigation.service';

@Component({
    selector: 'app-year-list',
    templateUrl: './year-list.component.html',
    styleUrls: [ './year-list.component.css' ],
    animations: [
        trigger('fadeInOut', [
            state('in', style({opacity: 1})),
            transition(':enter', useAnimation(fadeIn)),
            transition(':leave', useAnimation(fadeOut))
        ])
    ]
})
export class YearListComponent implements OnInit {
    year = -1;
    years$: Observable<number[]>;

    constructor(private _dataService: VideoDataService,
                private _navService: VideoNavigationService) {

    }

    selectItem(year: number): void {
        this._navService.gotoCategoryList(year);
    }

    ngOnInit(): void {
        this.years$ = this._dataService
            .getYears();
    }
}
