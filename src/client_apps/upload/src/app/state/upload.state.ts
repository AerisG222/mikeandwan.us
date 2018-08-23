import { State, Action, StateContext } from '@ngxs/store';
import { IFileInfo } from '../models/ifile-info';
import { UploadService } from '../services/upload.service';
import * as actions from './upload.actions';

export interface UploadStateModel {
    serverFiles: Array<IFileInfo>;
    error: any;
}

@State<UploadStateModel>({
    name: 'upload',
    defaults: {
        serverFiles: [],
        error: null
    }
})
export class UploadAppState {
    constructor(private _uploadService: UploadService) {

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
