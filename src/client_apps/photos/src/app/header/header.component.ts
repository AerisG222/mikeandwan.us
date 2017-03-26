import { Component, ViewChild, AfterViewInit } from '@angular/core';

import { BreadcrumbListComponent } from '../../ng_maw/breadcrumb-list/breadcrumb-list.component';
import { Breadcrumb } from '../../ng_maw/shared/breadcrumb.model';
import { SvgIcon } from '../../ng_maw/svg-icon/svg-icon.enum';

import { PhotoNavigationService } from '../shared/photo-navigation.service';
import { PhotoStateService } from '../shared/photo-state.service';
import { CategoryBreadcrumb } from '../shared/category-breadcrumb.model';

@Component({
    selector: 'app-header',
    templateUrl: './header.component.html',
    styleUrls: [ './header.component.css' ]
})
export class HeaderComponent implements AfterViewInit {
    svgIcon = SvgIcon;
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

    clickStats(): void {
        window.open('/photos/stats', '_blank');
    }

    clickAndroid(): void {
        window.open('https://play.google.com/store/apps/details?id=us.mikeandwan.pictures', '_blank');
    }

    private onBreadcrumbsUpdated(breadcrumbs: Array<Breadcrumb>): void {
        this.showMapButton = false;
        this.showRegularButton = false;
        this.downloadUrl = null;

        if (breadcrumbs != null && breadcrumbs.length > 0) {
            const dest = breadcrumbs[breadcrumbs.length - 1];

            if (dest instanceof CategoryBreadcrumb) {
                const cat = (<CategoryBreadcrumb>dest).category;

                this.downloadUrl = `/photos/download-category/${cat.id}`;

                if (cat.hasGpsData) {
                    this.showMapButton = true;
                }
            }
        }
    }
}
