import { Component, Input, HostBinding } from '@angular/core';

import { ICategory } from '../shared/icategory.model';

@Component({
    selector: 'app-category-card',
    templateUrl: './category-card.component.html',
    styleUrls: ['./category-card.component.css']
})
export class CategoryCardComponent {
    @HostBinding('class.card') cardClass = true;
    @Input() category: ICategory;
}
