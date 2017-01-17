import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';

import { BreadcrumbListComponent } from '../ng_maw/breadcrumb-list/breadcrumb-list.component';
import { DialogComponent } from '../ng_maw/dialog/dialog.component';
import { PagerComponent } from '../ng_maw/pager/pager.component';
import { ThumbnailListComponent } from '../ng_maw/thumbnail-list/thumbnail-list.component';
import { BreadcrumbService } from '../ng_maw/shared/breadcrumb.service';
import { LocalStorageService } from '../ng_maw/shared/local-storage.service';
import { ResponsiveService } from '../ng_maw/shared/responsive.service';

import { AppComponent } from './app.component';
import { CategoryListComponent } from './category-list/category-list.component';
import { HeaderComponent } from './header/header.component';
import { PreferenceDialogComponent } from './preference-dialog/preference-dialog.component';
import { VideoListComponent } from './video-list/video-list.component';
import { YearListComponent } from './year-list/year-list.component';
import { SizeService } from './shared/size.service';
import { VideoDataService } from './shared/video-data.service';
import { VideoNavigationService } from './shared/video-navigation.service';
import { VideoStateService } from './shared/video-state.service';

@NgModule({
    imports: [
        BrowserModule,
        FormsModule,
        HttpModule,
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
        DialogComponent,
        PagerComponent,
        ThumbnailListComponent,
        CategoryListComponent,
        HeaderComponent,
        PreferenceDialogComponent,
        VideoListComponent,
        YearListComponent
    ],
    providers: [
        BreadcrumbService,
        LocalStorageService,
        ResponsiveService,
        SizeService,
        VideoDataService,
        VideoNavigationService,
        VideoStateService
    ],
    bootstrap: [
        AppComponent
    ]
})
export class AppModule { }
