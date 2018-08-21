import { State, Action, StateContext } from '@ngxs/store';
import { IFileInfo } from '../shared/ifile-info';
import { DeleteServerFiles, LoadServerFiles } from './actions';

export interface UploadAppStateModel {
    serverFiles: Array<IFileInfo>;
}

@State<UploadAppStateModel>({
    name: 'uploadApp',
    defaults: {
        serverFiles: []
    }
})
export class UploadAppState {
    @Action(DeleteServerFiles)
    deleteServerFiles(ctx: StateContext<UploadAppStateModel>) {

    }

    @Action(LoadServerFiles)
    loadServerFiles(ctx: StateContext<UploadAppStateModel>) {

    }
}
