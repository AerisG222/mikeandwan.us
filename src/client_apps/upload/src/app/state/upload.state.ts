import { State, Action, StateContext, Selector, Store } from '@ngxs/store';
import { saveAs } from 'file-saver/FileSaver';
import { FileUploader } from 'ng2-file-upload';
import { tap } from 'rxjs/operators';

import { IFileInfo } from '../models/ifile-info';
import { UploadService } from '../services/upload.service';
import { AuthState } from './auth.state';
import {
    InitializeUploader,
    DeleteServerFiles,
    LoadServerFiles,
    LoadServerFilesSuccess,
    LoadServerFilesFailed,
    DownloadServerFiles,
    DownloadServerFilesError
} from './upload.actions';
import { HttpResponse } from '@angular/common/http';

export interface UploadStateModel {
    serverFiles: IFileInfo[];
    downloadError: any;
    error: any;
    uploader: FileUploader;
}

@State<UploadStateModel>({
    name: 'upload',
    defaults: {
        serverFiles: [],
        downloadError: null,
        error: null,
        uploader: null
    }
})
export class UploadState {
    private readonly _filenameRegex = /.*filename\=(.*);.*/;

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

    @Selector()
    static getDownloadError(state: UploadStateModel) {
        return state.downloadError;
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

    @Action(DownloadServerFiles)
    DownloadServerFiles(ctx: StateContext<UploadStateModel>, payload: DownloadServerFiles) {
        const list = [];

        list.push(payload.files);

        console.log(list);

        this._uploadService
            .downloadFiles(list)
            .subscribe(
                response => this.saveDownload(response),  // don't push such a big thing into state
                err => ctx.dispatch(new DownloadServerFilesError(err))
            );
    }

    @Action(DeleteServerFiles)
    deleteServerFiles(ctx: StateContext<UploadStateModel>, payload: DeleteServerFiles) {
        console.log(payload.files);
    }

    @Action(LoadServerFiles)
    loadServerFiles(ctx: StateContext<UploadStateModel>) {
        console.log('uploadstate.initUploader');

        this._uploadService
            .getServerFiles()
            .pipe(
                tap(x => console.log(x))
            )
            .subscribe(
                files => ctx.dispatch(new LoadServerFilesSuccess(files)),
                err => ctx.dispatch(new LoadServerFilesFailed(err))
            );
    }

    @Action(LoadServerFilesSuccess)
    loadServerFilesSuccess(ctx: StateContext<UploadStateModel>, payload: LoadServerFilesSuccess) {  // files: IFileInfo[]
        console.log('files:', payload.results);

        ctx.patchState({
            serverFiles: payload.results
        });
    }

    @Action(LoadServerFilesFailed)
    loadServerFilesFailed(ctx: StateContext<UploadStateModel>, payload: LoadServerFiles) {
        ctx.patchState({
            error: payload
        });
    }


    // we should probably not shove big things into state, so handle the downloaded content
    // here for now
    private saveDownload(response: HttpResponse<Blob>) {
        const disposition = response.headers.get('Content-Disposition');
        const results = this._filenameRegex.exec(disposition);
        const filename = results.length > 1 ? results[1] : 'download_file';

        saveAs(response.body, filename);
    }
}
