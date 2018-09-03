import { Injectable } from '@angular/core';
import { Store } from '@ngxs/store';
import { User } from 'oidc-client';
import { Observable, from } from 'rxjs';
import * as signalR from '@aspnet/signalr';

import { IFileInfo } from '../models/ifile-info';
import { EnvironmentConfig } from '../models/environment-config';

@Injectable({
    providedIn: 'root'
})
export class UploadService {
    private _hub: signalR.HubConnection;

    constructor(private _cfg: EnvironmentConfig,
                private _store: Store) {
        console.log('creating upload service');
        console.log(_store);
    }

    getServerFiles(): Observable<IFileInfo[]> {
        /* webapi call:
        const url = this.getAbsoluteUrl('upload/files');

        return this._http
            .get<Array<IFileInfo>>(url);
        */

        console.log('getserverfiles');

        const hub = this.getHubConnection();

        if (hub != null) {
            return from(this._hub.invoke('GetAllFiles'));
        }
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
    private getHubConnection() {
        if (!!this._hub === true) {
            return this._hub;
        }

        const userState = this._store.selectSnapshot(state => state.auth.user);

        if (!!userState === false || !!userState.user === false) {
            console.log('user is not defined, unable to get hub!');
            return null;
        }

        this.setupSignalrHub(userState.user);

        return this._hub;
    }

    private setupSignalrHub(user: User) {
        console.log('setting up signalr hub...');

        const tokenValue = `?token=${user.access_token}`;
        const url = `${this.getAbsoluteUrl('uploadr')}${tokenValue}`;

        console.log(url);

        this._hub = new signalR.HubConnectionBuilder()
            .withUrl(url)
            .configureLogging(signalR.LogLevel.Information)
            .build();

        this._hub.start().catch(err => console.error(err.toString()));

        this._hub.on('FileAdded', (uploadedFile: IFileInfo) => {
            // this._store.dispatch(new NewsActions.ReceivedItemAction(newsItem));
        });

        this._hub.on('FileDeleted', (uploadedFile: IFileInfo) => {
            // this._store.dispatch(new NewsActions.ReceivedGroupJoinedAction(data));
        });
    }
}
