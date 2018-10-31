import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { AppComponent } from './app.component';
import { BreadcrumbListComponent } from './breadcrumb-list/breadcrumb-list.component';
import { BreadcrumbService } from './services/breadcrumb.service';
import { CategoryCardComponent } from './category-card/category-card.component';
import { CategoryCardGridComponent } from './category-card-grid/category-card-grid.component';
import { CategoryListComponent } from './category-list/category-list.component';
import { HeaderComponent } from './header/header.component';
import { LocalStorageService } from './services/local-storage.service';
import { PreferenceDialogComponent } from './preference-dialog/preference-dialog.component';
import { VideoListComponent } from './video-list/video-list.component';
import { YearListComponent } from './year-list/year-list.component';
import { SizeService } from './services/size.service';
import { SvgIconComponent } from './svg-icon/svg-icon.component';
import { VideoCardComponent } from './video-card/video-card.component';
import { VideoCardGridComponent } from './video-card-grid/video-card-grid.component';
import { VideoDataService } from './services/video-data.service';
import { VideoStateService } from './services/video-state.service';
import { SignInComponent } from './sign-in/sign-in.component';
import { VideoNavigationService } from './services/video-navigation.service';
import { MawCommonModule, AuthGuardService, AuthInterceptor, AuthConfig, AuthService, EnvironmentConfig } from 'maw-common';

@NgModule({
    imports: [
        BrowserAnimationsModule,
        FormsModule,
        HttpClientModule,
        MawCommonModule,
        NgbModule,
        RouterModule.forRoot([
            { path: '',                component: YearListComponent,     canActivate: [AuthGuardService] },
            { path: 'signin-oidc',     component: SignInComponent },
            { path: ':year',           component: CategoryListComponent, canActivate: [AuthGuardService] },
            { path: ':year/:category', component: VideoListComponent,    canActivate: [AuthGuardService] },
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
        YearListComponent,
        SignInComponent
    ],
    providers: [
        BreadcrumbService,
        LocalStorageService,
        SizeService,
        VideoDataService,
        VideoNavigationService,
        VideoStateService,
        AuthService,
        AuthGuardService,
        EnvironmentConfig,
        {
            provide: AuthConfig,
            useFactory: (env: EnvironmentConfig) => {
                return new AuthConfig(
                    env.authUrl,
                    'maw_videos',
                    env.wwwUrl,
                    `${env.wwwUrl}/videos/signin-oidc`,
                    `${env.wwwUrl}/account/spa-silent-signin`
                );
            },
            deps: [ EnvironmentConfig ]
        },
        {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthInterceptor,
            multi: true
        }
    ],
    entryComponents: [
        PreferenceDialogComponent
    ],
    bootstrap: [
        AppComponent
    ]
})
export class AppModule { }
