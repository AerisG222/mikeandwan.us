import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { AppComponent } from './app.component';
import { AuthService } from './services/auth-service';
import { AuthGuardService } from './services/auth-guard.service';
import { AuthInterceptor } from './services/auth-interceptor';
import { BreadcrumbListComponent } from './breadcrumb-list/breadcrumb-list.component';
import { BreadcrumbService } from './services/breadcrumb.service';
import { CategoryLinkComponent } from './category-link/category-link.component';
import { CategoryListComponent } from './category-list/category-list.component';
import { CommentViewComponent } from './comment-view/comment-view.component';
import { EffectsViewComponent } from './effects-view/effects-view.component';
import { ExifTableComponent } from './exif-table/exif-table.component';
import { ExifViewComponent } from './exif-view/exif-view.component';
import { FullscreenViewComponent } from './fullscreen-view/fullscreen-view.component';
import { HeaderComponent } from './header/header.component';
import { HelpDialogComponent } from './help-dialog/help-dialog.component';
import { LocalStorageService } from './services/local-storage.service';
import { MapViewComponent } from './map-view/map-view.component';
import { ModeComponent } from './mode/mode.component';
import { PhotoDialogComponent } from './photo-dialog/photo-dialog.component';
import { PhotoListComponent } from './photo-list/photo-list.component';
import { PhotoViewComponent } from './photo-view/photo-view.component';
import { PreferenceDialogComponent } from './preference-dialog/preference-dialog.component';
import { RatingViewComponent } from './rating-view/rating-view.component';
import { ResponsiveService } from './services/responsive.service';
import { SaveDialogComponent } from './save-dialog/save-dialog.component';
import { SlideshowButtonComponent } from './slideshow-button/slideshow-button.component';
import { PhotoSourceFactory } from './models/photo-source-factory.model';
import { PhotoStateService } from './services/photo-state.service';
import { PhotoNavigationService } from './services/photo-navigation.service';
import { RouteMode } from './models/route-mode.model';
import { SvgIconComponent } from './svg-icon/svg-icon.component';
import { CategoryCardGridComponent } from './category-card-grid/category-card-grid.component';
import { CategoryCardComponent } from './category-card/category-card.component';
import { PhotoCardComponent } from './photo-card/photo-card.component';
import { PhotoCardGridComponent } from './photo-card-grid/photo-card-grid.component';
import { ToolbarButtonComponent } from './toolbar-button/toolbar-button.component';
import { SignInComponent } from './sign-in/sign-in.component';
import { ThreeDLinkComponent } from './three-dlink/three-dlink.component';
import { EnvironmentConfig } from './models/environment-config';
import { FocusRemoverDirective } from './focus-remover.directive';
import { PhotoDataService } from './services/photo-data.service';


// TODO: the odd constants for data below are to satisfy an AOT requirement - is there a better way?
//  SEE: https://github.com/angular/angular/issues/10789
@NgModule({
    imports: [
        BrowserAnimationsModule,
        FormsModule,
        HttpClientModule,
        NgbModule,
        RouterModule.forRoot([
            // tslint:disable-next-line:max-line-length
            { path: '',                     component: ModeComponent,         canActivate: [AuthGuardService], data: { animation: 'home' } },
            { path: 'signin-oidc',          component: SignInComponent },
            // tslint:disable-next-line:max-line-length
            { path: 'random',               component: PhotoListComponent,    canActivate: [AuthGuardService], data: { animation: 'random', mode: RouteMode.Random } },
            // tslint:disable-next-line:max-line-length
            { path: 'year/:year',           component: CategoryListComponent, canActivate: [AuthGuardService], data: { animation: 'years' } },
            // tslint:disable-next-line:max-line-length
            { path: 'year/:year/:category', component: PhotoListComponent,    canActivate: [AuthGuardService], data: { animation: 'categories', mode: RouteMode.Category } },
            // tslint:disable-next-line:max-line-length
            { path: 'comment/:type/:order', component: PhotoListComponent,    canActivate: [AuthGuardService], data: { animation: 'comments', mode: RouteMode.Comment } },
            // tslint:disable-next-line:max-line-length
            { path: 'rating/:type/:order',  component: PhotoListComponent,    canActivate: [AuthGuardService], data: { animation: 'rating', mode: RouteMode.Rating } },
            // tslint:disable-next-line:max-line-length
            { path: ':mode',                component: ModeComponent,         canActivate: [AuthGuardService], data: { animation: 'mode' } },
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
        ToolbarButtonComponent,
        SignInComponent,
        ThreeDLinkComponent,
        FocusRemoverDirective
    ],
    providers: [
        BreadcrumbService,
        LocalStorageService,
        ResponsiveService,
        PhotoDataService,
        PhotoSourceFactory,
        PhotoStateService,
        PhotoNavigationService,
        AuthService,
        AuthGuardService,
        EnvironmentConfig,
        {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthInterceptor,
            multi: true
        }
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
