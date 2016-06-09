import { Injectable } from '@angular/core';

@Injectable()
export class ResponsiveBreakpoint {
    constructor(public name: string, public width: number) { }
}
