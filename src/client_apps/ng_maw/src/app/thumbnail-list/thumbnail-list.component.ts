import { Component, Input, Output, EventEmitter } from '@angular/core';
import { trigger, query, useAnimation, stagger, state, style, animate, transition } from '@angular/animations';

import { fadeAnimation } from '../shared/animation';
import { ThumbnailInfo } from './thumbnail-info.model';
import { SelectedThumbnail } from './selected-thumbnail.model';
import { ResponsiveService } from '../shared/';
import { SvgIcon } from '../svg-icon/svg-icon.enum';

@Component({
    selector: 'maw-thumbnail-list',
    templateUrl: './thumbnail-list.component.html',
    styleUrls: [ './thumbnail-list.component.css' ]
})
export class ThumbnailListComponent {
    @Input() itemsPerPage = 3;

    svgIcon = SvgIcon;
    pageDisplayedIndex = 0;
    itemSelectedIndex = -1;
    itemList: Array<ThumbnailInfo> = [];
    activeItem: ThumbnailInfo = null;
    hoverItem: ThumbnailInfo = null;
    itemsPerPageUpdated = new EventEmitter<number>();
    @Output() selected = new EventEmitter<SelectedThumbnail>();

    constructor() {
        this.updateItemsPerRow();
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
}
