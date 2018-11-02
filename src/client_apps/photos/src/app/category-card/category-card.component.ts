import { Component, Input, HostBinding } from '@angular/core';

import { ICategory } from '../models/icategory.model';
import { SvgIcon } from 'maw-common';

@Component({
    selector: 'app-category-card',
    templateUrl: './category-card.component.html',
    styleUrls: ['./category-card.component.css']
})
export class CategoryCardComponent {
    @HostBinding('class.card') cardClass = true;
    @Input() category: ICategory;
    svgIcon = SvgIcon;
}
