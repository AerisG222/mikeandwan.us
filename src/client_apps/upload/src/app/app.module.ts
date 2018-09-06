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
import { AuthService } from './services/auth-service';
import { AuthGuardService } from './services/auth-guard.service';
import { EnvironmentConfig } from './models/environment-config';
import { AuthInterceptor } from './services/auth-interceptor';
import { SignInComponent } from './sign-in/sign-in.component';
import { HomeComponent } from './home/home.component';
import { FileListingComponent } from './file-listing/file-listing.component';
import { UploadComponent } from './upload/upload.component';
import { environment } from '../environments/environment';
import { UploadState } from './state/upload.state';
import { AuthState } from './state/auth.state';
import { FileSizePipe } from './pipes/file-size.pipe';
import { RelativeDatePipe } from './pipes/relative-date.pipe';
import { SvgIconComponent } from './svg-icon/svg-icon.component';
import { FocusRemoverDirective } from './directives/focus-remover.directive';
import { DownloadHandlerComponent } from './download-handler/download-handler.component';

@NgModule({
    imports: [
        BrowserAnimationsModule,
        FileUploadModule,
        FormsModule,
        HttpClientModule,
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
        // directives
        FocusRemoverDirective,

        // pipes
        FileSizePipe,
        RelativeDatePipe,

        // components
        AppComponent,
        FileListingComponent,
        HomeComponent,
        SignInComponent,
        SvgIconComponent,
        UploadComponent,
        DownloadHandlerComponent
    ],
    providers: [
        AuthService,
        AuthGuardService,
        EnvironmentConfig,
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
