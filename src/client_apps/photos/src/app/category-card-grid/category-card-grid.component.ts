import { Component, Input, EventEmitter, Output } from '@angular/core';

import { ICategory } from '../models/icategory.model';
import { CategoryIndex } from '../models/category-index.model';

@Component({
    selector: 'app-category-card-grid',
    templateUrl: './category-card-grid.component.html',
    styleUrls: ['./category-card-grid.component.css']
})
export class CategoryCardGridComponent {
    @Input() categoryList: Array<ICategory>;
    @Input() cardsPerPage: number;
    @Input() page: number;
    @Output() categorySelected = new EventEmitter<CategoryIndex>();

    selectCategory(item: ICategory) {
        const idx = this.categoryList.findIndex(x => x.id === item.id);

        this.categorySelected.emit(new CategoryIndex(idx, item));
    }
}
