import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import {NgbModule} from '@ng-bootstrap/ng-bootstrap';

import { AppComponent } from './app.component';
import { BreadcrumbListComponent } from './breadcrumb-list/breadcrumb-list.component';
import { BreadcrumbService } from './shared/breadcrumb.service';
import { CategoryLinkComponent } from './category-link/category-link.component';
import { CategoryListComponent } from './category-list/category-list.component';
import { CommentViewComponent } from './comment-view/comment-view.component';
import { EffectsViewComponent } from './effects-view/effects-view.component';
import { ExifTableComponent } from './exif-table/exif-table.component';
import { ExifViewComponent } from './exif-view/exif-view.component';
import { FullscreenViewComponent } from './fullscreen-view/fullscreen-view.component';
import { HeaderComponent } from './header/header.component';
import { HelpDialogComponent } from './help-dialog/help-dialog.component';
import { LocalStorageService } from './shared/local-storage.service';
import { MapViewComponent } from './map-view/map-view.component';
import { ModeComponent } from './mode/mode.component';
import { PhotoDialogComponent } from './photo-dialog/photo-dialog.component';
import { PhotoListComponent } from './photo-list/photo-list.component';
import { PhotoViewComponent } from './photo-view/photo-view.component';
import { PreferenceDialogComponent } from './preference-dialog/preference-dialog.component';
import { RatingViewComponent } from './rating-view/rating-view.component';
import { ResponsiveService } from './shared/responsive.service';
import { SaveDialogComponent } from './save-dialog/save-dialog.component';
import { SlideshowButtonComponent } from './slideshow-button/slideshow-button.component';
import { PhotoDataService } from './shared/photo-data.service';
import { PhotoSourceFactory } from './shared/photo-source-factory.model';
import { PhotoStateService } from './shared/photo-state.service';
import { PhotoNavigationService } from './shared/photo-navigation.service';
import { RouteMode } from './shared/route-mode.model';
import { SvgIconComponent } from './svg-icon/svg-icon.component';
import { CategoryCardGridComponent } from './category-card-grid/category-card-grid.component';
import { CategoryCardComponent } from './category-card/category-card.component';
import { PhotoCardComponent } from './photo-card/photo-card.component';
import { PhotoCardGridComponent } from './photo-card-grid/photo-card-grid.component';
import { ToolbarButtonComponent } from './toolbar-button/toolbar-button.component';

// TODO: the odd constants for data below are to satisfy an AOT requirement - is there a better way?
//  SEE: https://github.com/angular/angular/issues/10789
@NgModule({
    imports: [
        BrowserAnimationsModule,
        FormsModule,
        HttpClientModule,
        NgbModule.forRoot(),
        RouterModule.forRoot([
            { path: '',                     component: ModeComponent,         data: { animation: 'home' } },
            { path: 'random',               component: PhotoListComponent,    data: { animation: 'random', mode: RouteMode.Random } },
            { path: 'year/:year',           component: CategoryListComponent, data: { animation: 'years' } },
            { path: 'year/:year/:category', component: PhotoListComponent,    data: { animation: 'categories', mode: RouteMode.Category } },
            { path: 'comment/:type/:order', component: PhotoListComponent,    data: { animation: 'comments', mode: RouteMode.Comment } },
            { path: 'rating/:type/:order',  component: PhotoListComponent,    data: { animation: 'rating', mode: RouteMode.Rating } },
            { path: ':mode',                component: ModeComponent,         data: { animation: 'mode' } },
            { path: '**',                   redirectTo: '/' }
        ])
    ],
    declarations: [
        AppComponent,
        BreadcrumbListComponent,
        SvgIconComponent,
        CategoryCardComponent,
        CategoryCardGridComponent,
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
        PhotoCardComponent,
        PhotoCardGridComponent,
        PhotoDialogComponent,
        PhotoListComponent,
        PhotoViewComponent,
        PreferenceDialogComponent,
        RatingViewComponent,
        SaveDialogComponent,
        SlideshowButtonComponent,
        ToolbarButtonComponent
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
    entryComponents: [
        HelpDialogComponent,
        PhotoDialogComponent,
        PreferenceDialogComponent,
        SaveDialogComponent
    ],
    bootstrap: [
        AppComponent
    ]
})
export class AppModule { }
