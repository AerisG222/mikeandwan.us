import { Component, OnDestroy } from '@angular/core';

declare var notick: boolean;

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: [ './app.component.css' ]
})
export class AppComponent implements OnDestroy {
    intervalId: number;
    currentTime: Date;
    h: number;
    m: number;
    s: number;
    notick = false;

    constructor() {
        if (typeof(notick) !== 'undefined') {
            this.notick = notick;
        }

        if (this.notick) {
            this.updateDisplay(new Date(2016, 6, 26, 12, 56, 39));
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

        /* tslint:disable:no-bitwise */
        return (digit & compareBit) === compareBit;
        /* tslint:enable:no-bitwise */
    }

    isOff(value: number, compareBit: number, isTens: boolean): boolean {
        return !this.isOn(value, compareBit, isTens);
    }

    tick(): void {
        this.updateDisplay(new Date());
    }

    updateDisplay(theDate: Date): void {
        if (theDate.getSeconds() !== this.s) {
            this.currentTime = theDate;
            this.h = theDate.getHours();
            this.m = theDate.getMinutes();
            this.s = theDate.getSeconds();
        }
    }
}
