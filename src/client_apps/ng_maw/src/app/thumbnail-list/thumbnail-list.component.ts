import { Component, Output, EventEmitter } from '@angular/core';
import { trigger, query, useAnimation, stagger, state, style, animate, transition } from '@angular/animations';

import { fadeAnimation } from '../shared/animation';
import { ThumbnailInfo, SelectedThumbnail } from './';
import { ResponsiveService } from '../shared/';
import { SvgIcon } from '../svg-icon/svg-icon.enum';

@Component({
    selector: 'maw-thumbnail-list',
    templateUrl: './thumbnail-list.component.html',
    styleUrls: [ './thumbnail-list.component.css' ],
    animations: [
        trigger('fadeInOut', [
            transition('* => *', [
                // hide everything right away
                query(':enter',
                    style({ opacity: 0 }),
                    { optional: true }
                ),

                // now display in a staggered fashion
                query(':enter',
                    stagger('200ms', [
                        useAnimation(fadeAnimation, {
                            params: {
                                from: 0,
                                to: 1,
                                time: '320ms'
                            }
                        })
                    ]),
                    { optional: true }
                ),

                query(':leave',
                    stagger('250ms', [
                        useAnimation(fadeAnimation, {
                            params: {
                                from: 1,
                                to: 0,
                                time: '200ms'
                            }
                        })
                    ]),
                    { optional: true }
                )
            ])
        ])
    ]
})
export class ThumbnailListComponent {
    svgIcon = SvgIcon;
    rowsPerPage = 3;
    itemsPerRow = 6;
    pageDisplayedIndex = 0;
    itemSelectedIndex = -1;
    itemList: Array<ThumbnailInfo> = [];
    displayedRows: Array<Array<ThumbnailInfo>> = [];
    activeItem: ThumbnailInfo = null;
    hoverItem: ThumbnailInfo = null;
    itemsPerPageUpdated = new EventEmitter<number>();
    @Output() selected = new EventEmitter<SelectedThumbnail>();

    constructor(private _responsiveService: ResponsiveService) {
        this.updateItemsPerRow();

        this._responsiveService.onBreakpointChange.subscribe((breakpoint: string) => {
            this.handleResize(breakpoint);
        });
    }

    get itemsPerPage(): number {
        return this.itemsPerRow * this.rowsPerPage;
    }

    handleResize(newsize: string): void {
        this.updateItemsPerRow();
        this.updateView();
    }

    selectItem(item: ThumbnailInfo, rowIndex: number, itemIndex: number): void {
        if (this.activeItem !== item) {
            this.activeItem = item;
            this.itemSelectedIndex = (this.pageDisplayedIndex * this.itemsPerPage) + (rowIndex * this.itemsPerRow) + itemIndex;

            this.selected.next(new SelectedThumbnail(this.itemSelectedIndex, this.activeItem));
        }
    }

    setRowCountPerPage(count: number): void {
        if (count >= 0 && this.rowsPerPage !== count) {
            this.rowsPerPage = count;
            this.updateView();
            this.itemsPerPageUpdated.next(null);
        }
    }

    setItemSelectedIndex(index: number): void {
        if (index >= 0 && this.itemSelectedIndex !== index) {
            this.activeItem = this.itemList[index];
            this.itemSelectedIndex = index;
        }
    }

    setPageDisplayedIndex(index: number): void {
        if (index >= 0 && this.pageDisplayedIndex !== index) {
            this.pageDisplayedIndex = index;
            this.updateView();
        }
    }

    setItemList(list: Array<ThumbnailInfo>): void {
        this.itemList = list;
        this.updateView();
    }

    addItem(item: ThumbnailInfo): void {
        this.itemList.push(item);
        this.updateView();
    }

    updateView(): void {
        this.displayedRows = [];

        if (this.itemList.length === 0) {
            return;
        }

        const pageStartItemIndex = this.pageDisplayedIndex * this.itemsPerPage;

        for (let i = 0; i < this.rowsPerPage; i++) {
            const rowStartIndex = pageStartItemIndex + (this.itemsPerRow * i);

            if (rowStartIndex >= this.itemList.length) {
                return;
            }

            const endPosition = Math.min(rowStartIndex + this.itemsPerRow, this.itemList.length);

            this.displayedRows.push(this.itemList.slice(rowStartIndex, endPosition));
        }
    }

    private updateItemsPerRow(): void {
        const origItemsPerPage = this.itemsPerPage;

        if (this._responsiveService._currBp === ResponsiveService.BP_LG) {
            this.itemsPerRow = 6;
        } else {
            this.itemsPerRow = 4;
        }

        if (origItemsPerPage !== this.itemsPerPage) {
            // reset page displayed index to best match where the user left off
            const targetIndex = origItemsPerPage * this.pageDisplayedIndex;
            this.pageDisplayedIndex = Math.floor(targetIndex / this.itemsPerPage);

            this.itemsPerPageUpdated.next(null);
        }
    }
}
