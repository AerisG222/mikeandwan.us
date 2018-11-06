import { NgModule } from '@angular/core';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NgxsModule } from '@ngxs/store';
import { NgxsReduxDevtoolsPluginModule } from '@ngxs/devtools-plugin';
import { NgxsLoggerPluginModule } from '@ngxs/logger-plugin';
import { FileUploadModule } from 'ng2-file-upload';

import { AppComponent } from './app.component';
import { AuthStoreManagerService } from './services/auth-store-manager.service';
import { SignInComponent } from './sign-in/sign-in.component';
import { HomeComponent } from './home/home.component';
import { FileListingComponent } from './file-listing/file-listing.component';
import { UploadComponent } from './upload/upload.component';
import { environment } from '../environments/environment';
import { UploadState } from './state/upload.state';
import { AuthState } from './state/auth.state';
import { FileSizePipe } from './pipes/file-size.pipe';
import { RelativeDatePipe } from './pipes/relative-date.pipe';
import { DownloadHandlerComponent } from './download-handler/download-handler.component';
import { FileThumbnailComponent } from './file-thumbnail/file-thumbnail.component';
import { MawCommonModule, EnvironmentConfig, } from 'maw-common';
import { MawAuthModule, AuthConfig, AuthGuardService, AuthInterceptor, AuthService } from 'maw-auth';


@NgModule({
    imports: [
        BrowserAnimationsModule,
        FileUploadModule,
        FormsModule,
        HttpClientModule,
        MawAuthModule,
        MawCommonModule,
        // NgbModule.forRoot(),
        NgxsModule.forRoot([
            AuthState,
            UploadState
        ]),
        NgxsReduxDevtoolsPluginModule.forRoot({
            disabled: environment.production
        }),
        NgxsLoggerPluginModule.forRoot({
            disabled: environment.production
        }),
        RouterModule.forRoot([
            { path: '',            component: HomeComponent,  canActivate: [AuthGuardService] },
            { path: 'signin-oidc', component: SignInComponent },
            { path: '**',          redirectTo: '/' },
        ])
    ],
    declarations: [
        // pipes
        FileSizePipe,
        RelativeDatePipe,

        // components
        AppComponent,
        FileListingComponent,
        HomeComponent,
        SignInComponent,
        UploadComponent,
        DownloadHandlerComponent,
        FileThumbnailComponent
    ],
    providers: [
        AuthService,
        AuthGuardService,
        AuthStoreManagerService,
        EnvironmentConfig,
        {
            provide: AuthConfig,
            useFactory: (env: EnvironmentConfig) => {
                return new AuthConfig(
                    env.authUrl,
                    'maw_upload',
                    env.wwwUrl,
                    `${env.wwwUrl}/upload/signin-oidc`,
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
    bootstrap: [
        AppComponent
    ]
})
export class AppModule { }
