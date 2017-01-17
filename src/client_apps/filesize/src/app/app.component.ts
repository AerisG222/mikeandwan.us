import { Component } from '@angular/core';

import { Result } from './result.model';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: [ './app.component.css' ]
})
export class AppComponent {
    results: Result[] = null;

    sizes = [
        { label: 'B',  bytesInUnit: 1 },
        { label: 'KB', bytesInUnit: 1024 },
        { label: 'MB', bytesInUnit: 1024 * 1024 },
        { label: 'GB', bytesInUnit: 1024 * 1024 * 1024 }];

    calculate(size: number, sizeScale: string): void {
        let sizeInBytes = 0;
        const results: Result[] = [];
        let i = 0;

        for (i = 0; i < this.sizes.length; i++) {
            if (this.sizes[i].label.toLowerCase() === sizeScale) {
                sizeInBytes = size * this.sizes[i].bytesInUnit;
                break;
            }
        }

        for (i = 0; i < this.sizes.length; i++) {
            results.push(new Result(this.sizes[i].label, sizeInBytes / this.sizes[i].bytesInUnit));
        }

        this.results = results;
    }
}
