import { Component } from '@angular/core';
import { NgIf, NgFor } from '@angular/common';

import { Result } from './';

@Component({
    moduleId: module.id,
    selector: 'time-app',
    directives: [NgIf, NgFor],
    templateUrl: 'time.component.html',
    styleUrls: ['time.component.css']
})
export class TimeAppComponent {
    results: Result[] = null;
    sizes = [
        { label: 'seconds', timeInSeconds: 1 },
        { label: 'minutes', timeInSeconds: 60 },
        { label: 'hours',   timeInSeconds: 60 * 60 },
        { label: 'days',    timeInSeconds: 60 * 60 * 24 },
        { label: 'years',   timeInSeconds: 60 * 60 * 24 * 365.242 }];

    calculate(time: number, timeScale: string): void {
        let timeInSeconds: number = 0;
        let results: Result[] = [];
        let i: number = 0;

        for (i = 0; i < this.sizes.length; i++) {
            if (timeScale.toLowerCase() === this.sizes[i].label) {
                timeInSeconds = time * this.sizes[i].timeInSeconds;
            }
        }

        for (i = 0; i < this.sizes.length; i++) {
            results.push(new Result(this.sizes[i].label, timeInSeconds / this.sizes[i].timeInSeconds));
        }

        this.results = results;
    }
}
