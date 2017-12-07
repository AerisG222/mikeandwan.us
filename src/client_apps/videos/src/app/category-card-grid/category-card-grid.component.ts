import { Component, Input, EventEmitter, Output } from '@angular/core';

import { ICategory } from '../shared/icategory.model';

@Component({
    selector: 'app-category-card-grid',
    templateUrl: './category-card-grid.component.html',
    styleUrls: ['./category-card-grid.component.css']
})
export class CategoryCardGridComponent {
    @Input() categoryList: Array<ICategory>;
    @Input() cardsPerPage: number;
    @Input() page: number;
    @Input() activeItem: ICategory;
    @Output() categorySelected = new EventEmitter<ICategory>();

    selectCategory(item: ICategory) {
        this.categorySelected.emit(item);
    }
}
