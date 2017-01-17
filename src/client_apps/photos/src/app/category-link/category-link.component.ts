import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';

import { Breadcrumb } from '../../ng_maw/shared/breadcrumb.model';

import { ICategory } from '../shared/icategory.model';
import { PhotoNavigationService } from '../shared/photo-navigation.service';
import { RouteMode } from '../shared/route-mode.model';
import { CategoryBreadcrumb } from '../shared/category-breadcrumb.model';

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
        const b = new CategoryBreadcrumb(cat.name, [ '/year', cat.year, cat.id ], cat);

        this._navService.gotoCategoryPhotoList(b);
    }
}
