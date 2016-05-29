import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { ICategory, PhotoNavigationService, RouteMode, CategoryBreadcrumb } from '../shared';
import { Breadcrumb } from '../../../../ng_maw/src/app/shared';

@Component({
  moduleId: module.id,
  selector: 'app-category-link',
  templateUrl: 'category-link.component.html',
  styleUrls: ['category-link.component.css']
})
export class CategoryLinkComponent {
    @Input() category : ICategory;
    
    constructor(private _router : Router,
                private _navService : PhotoNavigationService) {
        
    }
    
    gotoYear(cat : ICategory) : void {
        let b = new Breadcrumb(cat.year.toString(), this._router.generate(['CategoryList', {year : cat.year}]));
        
        this._navService.gotoSpecificMode(b, RouteMode.Category);
    }
    
    gotoCategory(cat : ICategory) : void {
        let b = new CategoryBreadcrumb(cat.name, this._router.generate([ 'CategoryPhotoList', { year: cat.year, category: cat.id }]), cat);
        
        this._navService.gotoCategoryPhotoList(b);
    }
}
