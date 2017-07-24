import { Component } from '@angular/core';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: [ './app.component.css' ]
})
export class AppComponent {
    bCount = 0;
    kbCount = 0;
    mbCount = 0;

    calculate(value: string): void {
        this.bCount = value.length;
        this.kbCount = this.bCount / 1024;
        this.mbCount = this.bCount / (1024 * 1024);
    }
}
