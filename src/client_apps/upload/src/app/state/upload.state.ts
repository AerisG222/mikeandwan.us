import { State, Action, StateContext, Selector, Select, Store } from '@ngxs/store';
import { FileUploader } from 'ng2-file-upload';
import { User } from 'oidc-client';
import { Observable } from 'rxjs';

import { IFileInfo } from '../models/ifile-info';
import { UploadService } from '../services/upload.service';
import { AuthState } from './auth.state';
import {
    InitializeUploader,
    DeleteServerFiles,
    LoadServerFiles,
    LoadServerFilesSuccess,
    LoadServerFilesFailed
} from './upload.actions';

export interface UploadStateModel {
    serverFiles: Array<IFileInfo>;
    error: any;
    uploader: FileUploader;
}

@State<UploadStateModel>({
    name: 'upload',
    defaults: {
        serverFiles: [],
        error: null,
        uploader: null
    }
})
export class UploadState {
    constructor(private _uploadService: UploadService,
                private _store: Store) {

    }

    @Selector()
    static getServerFiles(state: UploadStateModel) {
        return state.serverFiles;
    }

    @Selector()
    static getUploader(state: UploadStateModel) {
        return state.uploader;
    }

    @Action(InitializeUploader)
    initUploader(ctx: StateContext<UploadStateModel>) {
        console.log('uploadstate.initUploader');

        const slice = <any> this._store.selectSnapshot(AuthState.getUser);

        let token: string = null;

        if (slice != null && slice !== undefined && slice.user != null && slice.user !== undefined) {
            token = slice.user.access_token;
        }

        ctx.patchState({
            uploader: new FileUploader({
                url: this._uploadService.getAbsoluteUrl('upload/upload'),
                authToken: `Bearer ${token}`
            })
        });
    }

    @Action(DeleteServerFiles)
    deleteServerFiles(ctx: StateContext<UploadStateModel>) {

    }

    @Action(LoadServerFiles)
    loadServerFiles(ctx: StateContext<UploadStateModel>) {
        console.log('uploadstate.initUploader');

        this._uploadService
            .getServerFiles()
            .subscribe(
                files => ctx.dispatch(new LoadServerFilesSuccess(files)),
                err => ctx.dispatch(new LoadServerFilesFailed(err))
            );
    }

    @Action(LoadServerFilesSuccess)
    loadServerFilesSuccess(ctx: StateContext<UploadStateModel>, files: Array<IFileInfo>) {
        ctx.patchState({
            serverFiles: files
        });
    }

    @Action(LoadServerFilesFailed)
    loadServerFilesFailed(ctx: StateContext<UploadStateModel>, error: any) {
        ctx.patchState({
            error: error
        });
    }
}
