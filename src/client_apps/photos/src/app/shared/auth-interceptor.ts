import { Observable } from 'rxjs/Observable';
import { Injectable } from '@angular/core';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';

import { AuthService } from './auth-service';

// https://blog.angular-university.io/angular-jwt-authentication/

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    constructor(private _auth: AuthService) {

    }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const idToken = this._auth.getToken();

        if (idToken) {
            req = req.clone({ headers: req.headers.set('Authorization', `Bearer ${idToken}`) });

            return next.handle(req);
        } else {
            this._auth.login();
            // return next.handle(req);
        }
    }
}
