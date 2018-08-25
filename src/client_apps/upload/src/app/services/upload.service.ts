import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IFileInfo } from '../models/ifile-info';
import { EnvironmentConfig } from '../models/environment-config';

@Injectable({
    providedIn: 'root'
})
export class UploadService {
    constructor(private _http: HttpClient,
                private _cfg: EnvironmentConfig) {

    }

    getServerFiles(): Observable<Array<IFileInfo>> {
        const url = this.getAbsoluteUrl('upload/files');

        return this._http
            .get<Array<IFileInfo>>(url);
    }

    getAbsoluteUrl(relativeUrl: string) {
        return `${this._cfg.apiUrl}/${relativeUrl}`;
    }
}
