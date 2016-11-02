import { NgModule }      from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule }   from '@angular/forms';
import { HttpModule }     from '@angular/http';
import { RouterModule }  from '@angular/router';

import { BreadcrumbListComponent } from '../ng_maw/breadcrumb-list/breadcrumb-list.component';
import { DialogComponent } from '../ng_maw/dialog/dialog.component';
import { PagerComponent } from '../ng_maw/pager/pager.component';
import { RatingComponent } from '../ng_maw/rating/rating.component';
import { ThumbnailListComponent } from '../ng_maw/thumbnail-list/thumbnail-list.component';
import { BreadcrumbService } from '../ng_maw/shared/breadcrumb.service';
import { LocalStorageService } from '../ng_maw/shared/local-storage.service';
import { ResponsiveService } from '../ng_maw/shared/responsive.service';

import { AppComponent }  from './app.component';
import { CategoryLinkComponent } from './category-link/category-link.component';
import { CategoryListComponent } from './category-list/category-list.component';
import { CommentViewComponent } from './comment-view/comment-view.component';
import { EffectsViewComponent } from './effects-view/effects-view.component';
import { ExifTableComponent } from './exif-table/exif-table.component';
import { ExifViewComponent } from './exif-view/exif-view.component';
import { FullscreenViewComponent } from './fullscreen-view/fullscreen-view.component';
import { HeaderComponent } from './header/header.component';
import { HelpDialogComponent } from './help-dialog/help-dialog.component';
import { MapViewComponent } from './map-view/map-view.component';
import { ModeComponent } from './mode/mode.component';
import { PhotoDialogComponent } from './photo-dialog/photo-dialog.component';
import { PhotoListComponent } from './photo-list/photo-list.component';
import { PhotoViewComponent } from './photo-view/photo-view.component';
import { PreferenceDialogComponent } from './preference-dialog/preference-dialog.component';
import { RatingViewComponent } from './rating-view/rating-view.component';
import { SaveDialogComponent } from './save-dialog/save-dialog.component';
import { SlideshowButtonComponent } from './slideshow-button/slideshow-button.component';
import { ModeRouteInfo } from './shared/mode-route-info.model';
import { RouteMode } from './shared/route-mode.model';
import { PhotoDataService } from './shared/photo-data.service';
import { PhotoSourceFactory } from './shared/photo-source-factory.model';
import { PhotoStateService } from './shared/photo-state.service';
import { PhotoNavigationService } from './shared/photo-navigation.service';

// TODO: the odd constants for data below are to satisfy an AOT requirement - is there a better way?
//  SEE: https://github.com/angular/angular/issues/10789
@NgModule({
    imports: [
        BrowserModule,
        FormsModule,
        HttpModule,
        RouterModule.forRoot([
            { path: '',                     component: ModeComponent },
            { path: 'random',               component: PhotoListComponent,   data: ModeRouteInfo.RANDOM },
            { path: 'year/:year',           component: CategoryListComponent },
            { path: 'year/:year/:category', component: PhotoListComponent,   data: ModeRouteInfo.CATEGORY },
            { path: 'comment/:type/:order', component: PhotoListComponent,   data: ModeRouteInfo.COMMENT },
            { path: 'rating/:type/:order',  component: PhotoListComponent,   data: ModeRouteInfo.RATING },
            { path: ':mode',                component: ModeComponent },
            { path: '**',                   redirectTo: '/' }
        ])
    ],
    declarations: [
        AppComponent,
        BreadcrumbListComponent,
        DialogComponent,
        PagerComponent,
        RatingComponent,
        ThumbnailListComponent,
        CategoryLinkComponent,
        CategoryListComponent,
        CommentViewComponent,
        EffectsViewComponent,
        ExifTableComponent,
        ExifViewComponent,
        FullscreenViewComponent,
        HeaderComponent,
        HelpDialogComponent,
        MapViewComponent,
        ModeComponent,
        PhotoDialogComponent,
        PhotoListComponent,
        PhotoViewComponent,
        PreferenceDialogComponent,
        RatingViewComponent,
        SaveDialogComponent,
        SlideshowButtonComponent
    ],
    providers: [
        BreadcrumbService,
        LocalStorageService,
        ResponsiveService,
        PhotoDataService,
        PhotoSourceFactory,
        PhotoStateService,
        PhotoNavigationService
    ],
    bootstrap: [
        AppComponent
    ]
})
export class AppModule { }