import { Injectable } from '@angular/core';
import { Store } from '@ngxs/store';
import { User } from 'oidc-client';
import { Observable, from, BehaviorSubject } from 'rxjs';
import { filter, switchMap } from 'rxjs/operators';
import * as signalR from '@aspnet/signalr';

import { IFileInfo } from '../models/ifile-info';
import { EnvironmentConfig } from '../models/environment-config';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { IFileOperationResult } from '../models/ifile-operation-result';
import { FileAdded, FileDeleted } from '../state/upload.actions';

@Injectable({
    providedIn: 'root'
})
export class UploadService {
    private _hubReady$ = new BehaviorSubject<signalR.HubConnection>(undefined);

    constructor(private _cfg: EnvironmentConfig,
                private _http: HttpClient,
                private _store: Store) {
        console.log('creating upload service');
        console.log(_store);
    }

    getServerFiles(): Observable<IFileInfo[]> {
        console.log('getserverfiles');

        this.ensureHubConnected();

        return this._hubReady$
            .pipe(
                filter(hub => !!hub === true),
                switchMap(hub => from(hub.invoke('GetAllFiles')))
            );
    }

    deleteFiles(files: string[]): Observable<IFileOperationResult[]> {
        if (!!files === false || files.length === 0) {
            return;
        }

        return this._hubReady$
            .pipe(
                filter(hub => !!hub === true),
                switchMap(hub => from(hub.invoke('DeleteFiles', files)))
            );
    }

    downloadFiles(files: string[]): Observable<HttpResponse<Blob>> {
        if (!!files === false || files.length === 0) {
            return;
        }

        const url = this.getAbsoluteUrl('upload/download');

        return this._http
            .post(url, files, { responseType: 'blob', observe: 'response' });
    }

    getAbsoluteUrl(relativeUrl: string) {
        return `${this._cfg.apiUrl}/${relativeUrl}`;
    }

    // TODO: determine if there is a more reactive way to get the hub connection
    // when we tried to do this the first time, by using the @Select getUser to get the
    // user, this did not work, because this service was created before the others, and we
    // never got the user coming through after subscribing.  we now assume that our method
    // is called only after auth, and we should have a valid user instance to pull from state
    // (which should be populated after our constructor completes)
    private async ensureHubConnected() {
        if (!!this._hubReady$.value === true) {
            return;
        }

        const user = this._store.selectSnapshot(state => state.auth.user);

        if (!!user === false) {
            console.log('user is not defined, unable to get hub!');
            return;
        }

        await this.setupSignalrHub(user);
    }

    private async setupSignalrHub(user: User) {
        console.log('setting up signalr hub...');

        const tokenValue = `?token=${user.access_token}`;
        const url = `${this.getAbsoluteUrl('uploadr')}${tokenValue}`;

        const hub = new signalR.HubConnectionBuilder()
            .withUrl(url)
            .configureLogging(signalR.LogLevel.Information)
            .build();

        hub.on('FileAdded', (addedFile: IFileInfo) => {
            console.log('file added: ', addedFile);

            this._store.dispatch(new FileAdded(addedFile));
        });

        hub.on('FileDeleted', (deletedFile: IFileInfo) => {
            console.log('file deleted: ', deletedFile);

            this._store.dispatch(new FileDeleted(deletedFile));
        });

        hub.start()
            .then(() => this._hubReady$.next(hub))
            .catch(err => console.error(err.toString()));
    }
}
