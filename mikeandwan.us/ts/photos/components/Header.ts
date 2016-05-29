import { Component, ViewChild, AfterViewInit } from '@angular/core';
import { NgIf } from '@angular/common';
import { BreadcrumbList } from '../../ng_maw/breadcrumbList/BreadcrumbList';
import { PhotoNavigationService } from '../services/PhotoNavigationService';
import { PhotoStateService } from '../services/PhotoStateService';
import { Breadcrumb } from '../../ng_maw/services/Breadcrumb';
import { CategoryBreadcrumb } from '../models/CategoryBreadcrumb';

@Component({
    selector: 'header',
    directives: [ BreadcrumbList, NgIf ],
    templateUrl: '/js/photos/components/Header.html'
})
export class Header implements AfterViewInit {
    @ViewChild(BreadcrumbList) breadcrumbList : BreadcrumbList;
    downloadUrl : string = null;
    showMapButton = false;
    showRegularButton = false;
        
    constructor(private _navService : PhotoNavigationService, 
                private _stateService : PhotoStateService) {
        
    }
    
    ngAfterViewInit() : void {
        this.breadcrumbList.breadcrumbsChangedEventEmitter.subscribe((breadcrumbs : Array<Breadcrumb>) => {
           this.onBreadcrumbsUpdated(breadcrumbs);
        });
    }
    
    toggleMapView() : void {
        if(this.showMapButton) {
            this.showMapButton = false;
            this.showRegularButton = true;
        }
        else {
            this.showMapButton = true;
            this.showRegularButton = false;
        }

        this._stateService.toggleMapsView(!this.showMapButton);
    }
    
    clickConfig() : void {
        this._stateService.showPreferencesDialog();
    };
    
    private onBreadcrumbsUpdated(breadcrumbs : Array<Breadcrumb>) : void {
        this.showMapButton = false;
        this.showRegularButton = false;
        this.downloadUrl = null;
        
        if(breadcrumbs != null && breadcrumbs.length > 0) {
            let dest = breadcrumbs[breadcrumbs.length - 1];
            
            if(dest instanceof CategoryBreadcrumb) {
                let cat = (<CategoryBreadcrumb>dest).category;
                
                this.downloadUrl = `/photos/download-category/${cat.id}`;
                
                if(cat.hasGpsData) {
                    this.showMapButton = true;
                }
            }
        }
    }
}
