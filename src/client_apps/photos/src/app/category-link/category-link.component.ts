import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';

import { Breadcrumb } from '../models/breadcrumb.model';
import { ICategory } from '../models/icategory.model';
import { PhotoNavigationService } from '../services/photo-navigation.service';
import { RouteMode } from '../models/route-mode.model';
import { CategoryBreadcrumb } from '../models/category-breadcrumb.model';

@Component({
    selector: 'app-category-link',
    templateUrl: './category-link.component.html',
    styleUrls: [ './category-link.component.css' ]
})
export class CategoryLinkComponent {
    @Input() category: ICategory;

    constructor(private _router: Router,
                private _navService: PhotoNavigationService) {

    }

    gotoYear(cat: ICategory): void {
        const b = new Breadcrumb(cat.year.toString(), [ '/year', cat.year ]);

        this._navService.gotoSpecificMode(b, RouteMode.Category);
    }

    gotoCategory(cat: ICategory): void {
        this._navService.gotoCategoryPhotoList(cat);
    }
}
