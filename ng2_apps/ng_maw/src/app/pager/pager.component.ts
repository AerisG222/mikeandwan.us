import { Component, Output, EventEmitter } from '@angular/core';
import { NgClass, NgFor, NgIf, SlicePipe } from '@angular/common';

import { ResponsiveService } from '../shared';

// TODO: consider adding a PagedThumbnailList class combining both of these components.
//       this will allow for the consumer to not care about coordinating the 2 together
//       as each consumer must today.  Alternatively, just add the pager in by default
//       as it will probably always be used together.

@Component({
    moduleId: module.id,
    selector: 'app-pager',
    directives: [NgClass, NgIf, NgFor],
    templateUrl: 'pager.component.html',
    styleUrls: ['pager.component.css']
})
export class PagerComponent {
    @Output() pageSelected: EventEmitter<number> = new EventEmitter<number>();
    maxPagesDisplayed: number = 10;
    pageCount: number = 0;
    activePageIndex: number = 0;
    firstPageIndex: number = 0;
    pageArray: Array<number> = [];

    constructor(private _responsiveService: ResponsiveService) {
        this.handleResize(_responsiveService.getBreakpoint());

        this._responsiveService.onBreakpointChange.subscribe((breakpoint: string) => {
            this.handleResize(breakpoint);
        });
    }

    get canScroll(): boolean {
        return this.pageCount > this.maxPagesDisplayed;
    }

    get isFirstPageVisible(): boolean {
        return this.firstPageIndex === 0;
    }

    get isLastPageVisible(): boolean {
        return this.pageCount <= this.firstPageIndex + this.maxPagesDisplayed;
    }

    scroll(direction: number): void {
        this.firstPageIndex += direction;
    }

    isPageActive(index: number): boolean {
        return this.activePageIndex === index;
    }

    handleResize(newsize: string) {
        if (newsize === ResponsiveService.BP_LG) {
            this.maxPagesDisplayed = 30;
        }
        else if (newsize === ResponsiveService.BP_MD) {
            this.maxPagesDisplayed = 22;
        }
        else if (newsize === ResponsiveService.BP_SM) {
            this.maxPagesDisplayed = 16;
        }
        else {
            this.maxPagesDisplayed = 6;
        }
    }

    setPageCount(pageCount: number): void {
        if (pageCount >= 0 && (pageCount !== this.pageCount || this.pageArray.length !== pageCount)) {
            this.pageCount = pageCount;

            if (this.pageArray.length > this.pageCount) {
                while (this.pageArray.length > this.pageCount) {
                    this.pageArray.pop();
                }
            }
            else {
                while (this.pageArray.length < this.pageCount) {
                    this.pageArray.push(this.pageArray.length + 1);
                }
            }
        }
    }

    activatePage(index: number): void {
        if (this.setActivePage(index)) {
            this.pageSelected.next(index);
        }
    }

    setActivePage(index: number): boolean {
        if (index >= 0 && index < this.pageCount && index !== this.activePageIndex) {
            this.activePageIndex = index;
            return true;
        }

        return false;
    }

    calcPageCount(totalItemCount: number, itemCountPerPage: number): number {
        return Math.ceil(totalItemCount / itemCountPerPage);
    }

    calcActivePage(itemIndex: number, itemCountPerPage: number): number {
        return Math.floor(itemIndex / itemCountPerPage);
    }
}
