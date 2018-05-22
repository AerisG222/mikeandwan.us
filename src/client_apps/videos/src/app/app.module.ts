import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { AuthGuardService } from './shared/auth-guard.service';
import { AppComponent } from './app.component';
import { AuthService } from './shared/auth-service';
import { AuthInterceptor } from './shared/auth-interceptor';
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
import { SignInComponent } from './sign-in/sign-in.component';
import { EnvironmentConfig } from './shared/environment-config';
import { FocusRemoverDirective } from './focus-remover.directive';

@NgModule({
    imports: [
        BrowserAnimationsModule,
        FormsModule,
        HttpClientModule,
        NgbModule.forRoot(),
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
        SignInComponent,
        FocusRemoverDirective
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
