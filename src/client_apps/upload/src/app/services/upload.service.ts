import { Injectable } from '@angular/core';
import { Store } from '@ngxs/store';
import { User } from 'oidc-client';
import { Observable, from, BehaviorSubject } from 'rxjs';
import { filter, switchMap } from 'rxjs/operators';
import * as signalR from '@aspnet/signalr';

import { IFileInfo } from '../models/ifile-info';
import { EnvironmentConfig } from '../models/environment-config';
import { HttpClient } from '@angular/common/http';

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

    downloadFiles(files: string[]): Observable<Blob> {
        if (!!files === false || files.length === 0) {
            return;
        }

        const url = this.getAbsoluteUrl('upload/download');

        return this._http
            .post<Blob>(url, files);
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

        hub.on('FileAdded', (uploadedFile: IFileInfo) => {
            // this._store.dispatch(new NewsActions.ReceivedItemAction(newsItem));
        });

        hub.on('FileDeleted', (uploadedFile: IFileInfo) => {
            // this._store.dispatch(new NewsActions.ReceivedGroupJoinedAction(data));
        });

        hub.start()
            .then(() => this._hubReady$.next(hub))
            .catch(err => console.error(err.toString()));
    }
}
