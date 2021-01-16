import { Component, OnDestroy } from '@angular/core';

declare let notick: boolean;

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: [ './app.component.scss' ]
})
export class AppComponent implements OnDestroy {
    intervalId = -1;
    currentTime = new Date();
    h = 0;
    m = 0;
    s = 0;
    notick = false;

    constructor() {
        // get the global value specified on the page
        if (typeof(notick) !== 'undefined') {
            this.notick = notick;
        }

        if (this.notick) {
            this.setDate(new Date(2016, 6, 26, 12, 56, 39));
        } else {
            this.tick();
            this.intervalId = window.setInterval(() => this.tick(), 300);
        }
    }

    ngOnDestroy(): void {
        clearInterval(this.intervalId);
    }

    isOn(value: number, compareBit: number, isTens: boolean): boolean {
        const position = isTens ? 1 : 0;
        const paddedValue = `0${value}`;  // either will be 0x or 0xx
        const digit = parseInt(paddedValue.charAt(paddedValue.length - 1 - position), 10);

        /* eslint-disable no-bitwise */
        return (digit & compareBit) === compareBit;
        /* eslint-enable no-bitwise */
    }

    isOff(value: number, compareBit: number, isTens: boolean): boolean {
        return !this.isOn(value, compareBit, isTens);
    }

    tick(): void {
        this.setDate(new Date());
    }

    setDate(theDate: Date): void {
        this.currentTime = theDate;
        this.h = theDate.getHours();
        this.m = theDate.getMinutes();
        this.s = theDate.getSeconds();
    }
}
