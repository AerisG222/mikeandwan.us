import { ApplicationRef, Injectable, EventEmitter } from '@angular/core';

import { ResponsiveBreakpoint } from './responsive-breakpoint.model';

@Injectable()
export class ResponsiveService {
    static readonly BP_XS = 'xs';
    static readonly BP_SM = 'sm';
    static readonly BP_MD = 'md';
    static readonly BP_LG = 'lg';

    onBreakpointChange: EventEmitter<string> = new EventEmitter<string>();
    breakpoints = [
        new ResponsiveBreakpoint(ResponsiveService.BP_XS, 0),
        new ResponsiveBreakpoint(ResponsiveService.BP_SM, 768),
        new ResponsiveBreakpoint(ResponsiveService.BP_MD, 992),
        new ResponsiveBreakpoint(ResponsiveService.BP_LG, 1200)];
    _currBp: string;

    constructor(private _applicationRef: ApplicationRef) {
        this._currBp = this.getBreakpoint();

        window.onresize = ((evt: UIEvent) => {
            const bp = this.getBreakpoint();

            if (this._currBp !== bp) {
                this._currBp = bp;
                this.onBreakpointChange.next(bp);
                this._applicationRef.tick();
            }
        });
    }

    getBreakpoint(): string {
        const width = this.getWidth();

        for (let i = 1; i < this.breakpoints.length; i++) {
            if (width < this.breakpoints[i].width) {
                return this.breakpoints[i - 1].name;
            }
        }

        return this.breakpoints[this.breakpoints.length - 1].name;
    }

    getWidth(): number {
        let width: number;

        if (document.body && document.body.offsetWidth) {
            width = document.body.offsetWidth;
        }
        if (document.compatMode === 'CSS1Compat' && document.documentElement && document.documentElement.offsetWidth) {
            width = document.documentElement.offsetWidth;
        }
        if (window.innerWidth && window.innerHeight) {
            width = window.innerWidth;
        }

        return width;
    }

    getHeight(): number {
        let height: number;

        if (document.body && document.body.offsetHeight) {
            height = document.body.offsetHeight;
        }
        if (document.compatMode === 'CSS1Compat' && document.documentElement && document.documentElement.offsetHeight) {
            height = document.documentElement.offsetHeight;
        }
        if (window.innerWidth && window.innerHeight) {
            height = window.innerHeight;
        }

        return height;
    }
}
