import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { AppComponent } from './app.component';
import { BreadcrumbListComponent } from './breadcrumb-list/breadcrumb-list.component';
import { BreadcrumbService } from './shared/breadcrumb.service';
import { CategoryCardComponent } from './category-card/category-card.component';
import { CategoryCardGridComponent } from './category-card-grid/category-card-grid.component';
import { CategoryListComponent } from './category-list/category-list.component';
import { HeaderComponent } from './header/header.component';
import { LocalStorageService } from './shared/local-storage.service';
import { PreferenceDialogComponent } from './preference-dialog/preference-dialog.component';
import { VideoListComponent } from './video-list/video-list.component';
import { YearListComponent } from './year-list/year-list.component';
import { SizeService } from './shared/size.service';
import { SvgIconComponent } from './svg-icon/svg-icon.component';
import { VideoCardComponent } from './video-card/video-card.component';
import { VideoCardGridComponent } from './video-card-grid/video-card-grid.component';
import { VideoDataService } from './shared/video-data.service';
import { VideoNavigationService } from './shared/video-navigation.service';
import { VideoStateService } from './shared/video-state.service';

@NgModule({
    imports: [
        BrowserAnimationsModule,
        FormsModule,
        HttpClientModule,
        NgbModule.forRoot(),
        RouterModule.forRoot([
            { path: '',                component: YearListComponent },
            { path: ':year',           component: CategoryListComponent },
            { path: ':year/:category', component: VideoListComponent },
            { path: '**',              redirectTo: '/' },
        ])
    ],
    declarations: [
        AppComponent,
        BreadcrumbListComponent,
        CategoryCardComponent,
        CategoryCardGridComponent,
        SvgIconComponent,
        CategoryListComponent,
        HeaderComponent,
        PreferenceDialogComponent,
        VideoCardComponent,
        VideoCardGridComponent,
        VideoListComponent,
        YearListComponent
    ],
    providers: [
        BreadcrumbService,
        LocalStorageService,
        SizeService,
        VideoDataService,
        VideoNavigationService,
        VideoStateService
    ],
    entryComponents: [
        PreferenceDialogComponent
    ],
    bootstrap: [
        AppComponent
    ]
})
export class AppModule { }
