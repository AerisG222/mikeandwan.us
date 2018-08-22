import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { NgxsModule } from '@ngxs/store';
import { NgxsReduxDevtoolsPluginModule } from '@ngxs/devtools-plugin';
import { NgxsLoggerPluginModule } from '@ngxs/logger-plugin';

import { AppComponent } from './app.component';
import { AuthService } from './services/auth-service';
import { AuthGuardService } from './services/auth-guard.service';
import { EnvironmentConfig } from './models/environment-config';
import { AuthInterceptor } from './services/auth-interceptor';
import { SignInComponent } from './sign-in/sign-in.component';
import { HomeComponent } from './home/home.component';
import { FileListingComponent } from './file-listing/file-listing.component';
import { UploadComponent } from './upload/upload.component';
import { NotificationsComponent } from './notifications/notifications.component';
import { environment } from '../environments/environment';

@NgModule({
    imports: [
        BrowserAnimationsModule,
        FormsModule,
        HttpClientModule,
        // NgbModule.forRoot(),
        NgxsModule.forRoot([]),
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
        AppComponent,
        FileListingComponent,
        HomeComponent,
        NotificationsComponent,
        SignInComponent,
        UploadComponent
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
