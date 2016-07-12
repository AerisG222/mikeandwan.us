import { Component, Input, AfterViewInit, OnDestroy } from '@angular/core';
import { NgClass } from '@angular/common';

@Component({
    moduleId: module.id,
    selector: 'binary-clock-app',
    directives: [ NgClass ],
    templateUrl: 'app.component.html',
    styleUrls: [ 'app.component.css' ]
})
export class BinaryClockAppComponent implements AfterViewInit, OnDestroy {
    intervalId: number;
    currentTime: Date;
    h: number;
    m: number;
    s: number;
    @Input() notick = false;

    ngAfterViewInit(): void {
        if (this.notick) {
            this.updateDisplay(new Date(2016, 6, 26, 12, 56, 39));
        } else {
            this.tick();
            this.intervalId = setInterval(() => {
                this.tick();
            }, 300);
        }
    }

    ngOnDestroy(): void {
        clearInterval(this.intervalId);
    }

    isOn(value: number, compareBit: number, isTens: boolean): boolean {
        let position = isTens ? 1 : 0;
        let paddedValue = `0${value}`;  // either will be 0x or 0xx
        let digit = parseInt(paddedValue.charAt(paddedValue.length - 1 - position), 10);

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
