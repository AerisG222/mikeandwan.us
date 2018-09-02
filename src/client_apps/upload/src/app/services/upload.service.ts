import { Injectable } from '@angular/core';
import { Select, Store } from '@ngxs/store';
import { User } from 'oidc-client';
import { Observable,  BehaviorSubject, from, Subject } from 'rxjs';
import { filter, tap, map, switchMap, first, catchError } from 'rxjs/operators';
import * as signalR from '@aspnet/signalr';

import { IFileInfo } from '../models/ifile-info';
import { EnvironmentConfig } from '../models/environment-config';
import { AuthState } from '../state/auth.state';

@Injectable({
    providedIn: 'root'
})
export class UploadService {
    private _initSubject$ = new Subject();
    private _hub: signalR.HubConnection;
    private _x: any;

    @Select(AuthState.getUser) private _user$: Observable<User>;

    constructor(private _cfg: EnvironmentConfig,
                private _store: Store) {
        this._x = this._user$
            .pipe(
                tap(x => console.log('user: ', x)),
                filter(user => user !== undefined)
            )
            .subscribe(
                user => this.setupSignalrHub(user),
                err => console.log(err)
            );
    }

    getServerFiles(): Observable<IFileInfo[]> {
        /* webapi call:
        const url = this.getAbsoluteUrl('upload/files');

        return this._http
            .get<Array<IFileInfo>>(url);
        */

        return this._user$
            .pipe(
                switchMap((user, idx) =>
                    from(this._hub
                        .invoke('GetAllFiles')
                    )
                )
            );
    }

    getAbsoluteUrl(relativeUrl: string) {
        return `${this._cfg.apiUrl}/${relativeUrl}`;
    }

    private setupSignalrHub(user: User) {
        // tslint:disable-next-line:no-console
        console.log('setting up signalr hub...');

        const tokenValue = `?token=${user.access_token}`;

        this._hub = new signalR.HubConnectionBuilder()
            .withUrl(`${this.getAbsoluteUrl('uploadr')}${tokenValue}`)
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
