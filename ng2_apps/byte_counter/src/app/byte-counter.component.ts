import { Component } from '@angular/core';

@Component({
    moduleId: module.id,
    selector: 'byte-counter-app',
    templateUrl: 'byte-counter.component.html',
    styleUrls: [ 'byte-counter.component.css' ]
})
export class ByteCounterAppComponent {
    bCount: number = 0;
    kbCount: number = 0;
    mbCount: number = 0;

    calculate(value: string): void {
        this.bCount = value.length;
        this.kbCount = this.bCount / 1024;
        this.mbCount = this.bCount / (1024 * 1024);
    };
}
