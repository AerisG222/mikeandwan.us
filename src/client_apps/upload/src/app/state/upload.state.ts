import { State, Action, StateContext, Selector } from '@ngxs/store';
import { IFileInfo } from '../models/ifile-info';
import { UploadService } from '../services/upload.service';
import * as actions from './upload.actions';

export interface UploadStateModel {
    serverFiles: Array<IFileInfo>;
    showUsername: boolean;
    error: any;
}

@State<UploadStateModel>({
    name: 'upload',
    defaults: {
        serverFiles: [],
        showUsername: false,
        error: null
    }
})
export class UploadState {
    constructor(private _uploadService: UploadService) {

    }

    @Selector()
    static getServerFiles(state: UploadStateModel) {
        return state.serverFiles;
    }

    @Selector()
    static getShowUsername(state: UploadStateModel) {
        return state.showUsername;
    }

    @Action(actions.DeleteServerFiles)
    deleteServerFiles(ctx: StateContext<UploadStateModel>) {

    }

    @Action(actions.LoadServerFiles)
    loadServerFiles(ctx: StateContext<UploadStateModel>) {
        this._uploadService
            .getServerFiles()
            .subscribe(
                x => ctx.dispatch(new actions.LoadServerFilesSuccess(x)),
                err => ctx.dispatch(new actions.LoadServerFilesFailed(err))
            );
    }

    @Action(actions.LoadServerFilesSuccess)
    loadServerFilesSuccess(ctx: StateContext<UploadStateModel>, files: Array<IFileInfo>) {
        ctx.patchState({
            serverFiles: files
        });
    }

    @Action(actions.LoadServerFilesFailed)
    loadServerFilesFailed(ctx: StateContext<UploadStateModel>, error: any) {
        ctx.patchState({
            error: error
        });
    }
}
