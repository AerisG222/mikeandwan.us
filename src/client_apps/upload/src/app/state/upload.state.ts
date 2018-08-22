import { State, Action, StateContext } from '@ngxs/store';
import { IFileInfo } from '../shared/ifile-info';
import { DeleteServerFiles, LoadServerFiles } from './upload.actions';

export interface UploadStateModel {
    serverFiles: Array<IFileInfo>;
}

@State<UploadStateModel>({
    name: 'upload',
    defaults: {
        serverFiles: []
    }
})
export class UploadAppState {
    @Action(DeleteServerFiles)
    deleteServerFiles(ctx: StateContext<UploadStateModel>) {

    }

    @Action(LoadServerFiles)
    loadServerFiles(ctx: StateContext<UploadStateModel>) {

    }
}
