import { Component, Input } from '@angular/core';
import { Router } from '@angular/router-deprecated';
import { ICategory } from '../interfaces/ICategory';
import { PhotoNavigationService } from '../services/PhotoNavigationService';
import { RouteMode } from '../models/RouteMode';
import { Breadcrumb } from '../../ng_maw/services/Breadcrumb';
import { CategoryBreadcrumb } from '../models/CategoryBreadcrumb';

@Component({
    selector: 'categorylink',
    templateUrl: '/js/photos/components/CategoryLink.html'
})
export class CategoryLink {
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
