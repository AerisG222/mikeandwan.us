import { Component, ViewChild, AfterViewInit } from '@angular/core';

import { BreadcrumbListComponent } from '../../ng_maw/breadcrumb-list/breadcrumb-list.component';
import { Breadcrumb } from '../../ng_maw/shared/breadcrumb.model';

import { PhotoNavigationService } from '../shared/photo-navigation.service';
import { PhotoStateService } from '../shared/photo-state.service';
import { CategoryBreadcrumb } from '../shared/category-breadcrumb.model';

@Component({
    selector: 'header',
    templateUrl: 'header.component.html',
    styleUrls: [ 'header.component.css' ]
})
export class HeaderComponent implements AfterViewInit {
    @ViewChild(BreadcrumbListComponent) breadcrumbList: BreadcrumbListComponent;
    downloadUrl: string = null;
    showMapButton = false;
    showRegularButton = false;

    constructor(private _navService: PhotoNavigationService,
                private _stateService: PhotoStateService) {

    }

    ngAfterViewInit(): void {
        this.breadcrumbList.breadcrumbsChangedEventEmitter.subscribe((breadcrumbs: Array<Breadcrumb>) => {
            this.onBreadcrumbsUpdated(breadcrumbs);
        });
    }

    toggleMapView(): void {
        if (this.showMapButton) {
            this.showMapButton = false;
            this.showRegularButton = true;
        } else {
            this.showMapButton = true;
            this.showRegularButton = false;
        }

        this._stateService.toggleMapsView(!this.showMapButton);
    }

    clickConfig(): void {
        this._stateService.showPreferencesDialog();
    };

    private onBreadcrumbsUpdated(breadcrumbs: Array<Breadcrumb>): void {
        this.showMapButton = false;
        this.showRegularButton = false;
        this.downloadUrl = null;

        if (breadcrumbs != null && breadcrumbs.length > 0) {
            let dest = breadcrumbs[breadcrumbs.length - 1];

            if (dest instanceof CategoryBreadcrumb) {
                let cat = (<CategoryBreadcrumb>dest).category;

                this.downloadUrl = `/photos/download-category/${cat.id}`;

                if (cat.hasGpsData) {
                    this.showMapButton = true;
                }
            }
        }
    }
}
