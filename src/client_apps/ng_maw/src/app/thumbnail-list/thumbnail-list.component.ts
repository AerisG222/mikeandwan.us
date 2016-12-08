import { Component, Output, EventEmitter } from '@angular/core';

import { ThumbnailInfo, SelectedThumbnail } from './';
import { ResponsiveService } from '../shared/';

@Component({
    selector: 'maw-thumbnail-list',
    templateUrl: './thumbnail-list.component.html',
    styleUrls: [ './thumbnail-list.component.css' ]
})
export class ThumbnailListComponent {
    rowsPerPage: number = 3;
    itemsPerRow: number = 6;
    pageDisplayedIndex: number = 0;
    itemSelectedIndex: number = -1;
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

        let pageStartItemIndex = this.pageDisplayedIndex * this.itemsPerPage;

        for (let i = 0; i < this.rowsPerPage; i++) {
            let rowStartIndex = pageStartItemIndex + (this.itemsPerRow * i);

            if (rowStartIndex >= this.itemList.length) {
                return;
            }

            let endPosition = Math.min(rowStartIndex + this.itemsPerRow, this.itemList.length);

            this.displayedRows.push(this.itemList.slice(rowStartIndex, endPosition));
        }
    }

    private updateItemsPerRow(): void {
        let origItemsPerPage = this.itemsPerPage;

        if (this._responsiveService._currBp === ResponsiveService.BP_LG) {
            this.itemsPerRow = 6;
        } else {
            this.itemsPerRow = 4;
        }

        if (origItemsPerPage !== this.itemsPerPage) {
            // reset page displayed index to best match where the user left off
            let targetIndex = origItemsPerPage * this.pageDisplayedIndex;
            this.pageDisplayedIndex = Math.floor(targetIndex / this.itemsPerPage);

            this.itemsPerPageUpdated.next(null);
        }
    }
}
