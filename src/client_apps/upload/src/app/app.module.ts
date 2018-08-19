import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { AuthService } from './shared/auth-service';
import { AuthGuardService } from './shared/auth-guard.service';
import { EnvironmentConfig } from './shared/environment-config';
import { AuthInterceptor } from './shared/auth-interceptor';
import { SignInComponent } from './sign-in/sign-in.component';
import { HomeComponent } from './home/home.component';
import { ResultComponent } from './result/result.component';
import { FileListingComponent } from './file-listing/file-listing.component';
import { UploadComponent } from './upload/upload.component';

@NgModule({
    imports: [
        BrowserAnimationsModule,
        FormsModule,
        HttpClientModule,
        // NgbModule.forRoot(),
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
        ResultComponent,
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
