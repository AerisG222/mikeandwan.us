import { Component, Input } from '@angular/core';

import { ICategory } from '../shared/icategory.model';

@Component({
    selector: 'app-category-card',
    templateUrl: './category-card.component.html',
    styleUrls: ['./category-card.component.css']
})
export class CategoryCardComponent {
    @Input() category: ICategory;
}
