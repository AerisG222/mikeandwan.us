import { Injectable } from '@angular/core';
import { Router, Instruction } from '@angular/router-deprecated';

import { BreadcrumbService, Breadcrumb } from '../../../../ng_maw/src/app/shared/index';

import { ICategory } from './index';


@Injectable()
export class VideoNavigationService {
	constructor(private _router : Router,
                private _navService : BreadcrumbService) {

	}
	
	gotoYearList() : void {
        let ins = this._router.generate(['YearList']);
        
        this.gotoDestination(ins, this.getRootBreadcrumbs());
	}
	
	gotoCategoryList(year : number) : void {
        let ins = this._router.generate(['CategoryList', {year: year}]);
        let bcs = this.getYearListBreadcrumbs(year);
        
		this.gotoDestination(ins, bcs);
	}
	
	gotoVideoList(year : number, category : ICategory) : void {
        let ins = this._router.generate(['VideoList', {year: year, category: category.id}]);
        let bcs = this.getCategoryListBreadcrumbs(year, category);
        
        this.gotoDestination(ins, bcs);
	}
    
    private getRootBreadcrumbs() : Array<Breadcrumb> {
        return [];
    }
    
    private getYearListBreadcrumbs(year : number) : Array<Breadcrumb> {
        let crumbs : Array<Breadcrumb> = [];
        
        crumbs.push(this.getYearListBreadcrumb(year));
        
        return crumbs;
    }

    private getCategoryListBreadcrumbs(year : number, category : ICategory) : Array<Breadcrumb> {
        let crumbs : Array<Breadcrumb> = [];
        
        crumbs.push(this.getYearListBreadcrumb(year));
        crumbs.push(this.getCategoryListBreadcrumb(year, category));
        
        return crumbs;
    }
    
    private getYearListBreadcrumb(year : number) : Breadcrumb {
        return new Breadcrumb(year.toString(), this._router.generate(['YearList']));
    }
    
    private getCategoryListBreadcrumb(year : number, category : ICategory) : Breadcrumb {
        return new Breadcrumb(category.name, this._router.generate(['CategoryList', {year: year}]));
    }
    
    private gotoDestination(instruction : Instruction, breadcrumbs : Array<Breadcrumb>) : void {
		this._router.navigateByInstruction(instruction).then(
			(data : any) => this._navService.setBreadcrumbs(breadcrumbs)
		);
    }
}
