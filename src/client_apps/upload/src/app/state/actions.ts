import { IFileInfo } from '../shared/ifile-info';
import { IFileOperationResult } from '../shared/ifile-operation-result';

export enum ActionTypes {
    DeleteServerFiles = 'Delete Server Files',
    DeleteServerFilesSuccess = 'Delete Server Files [success]',
    DeleteServerFilesFailure = 'Delete Server Files [failure]',

    LoadServerFiles = 'Load Server Files',
    LoadServerFilesSuccess = 'Load Server Files [success]',
    LoadServerFilesFailure = 'Load Server Files [failure]',

    UploadFiles = 'Upload Files',
    UploadFilesSuccess = 'Upload Files [success]',
    UploadFilesFailure = 'Upload Files [failure]'
}

export class DeleteServerFiles {
    static readonly type = '[File Listing] Delete Server Files';

    constructor(public files: Array<string>) { }
}

export class DeleteServerFilesFailed {
    static readonly type = '[API] Delete Server Files Failed';
}

export class DeleteServerFilesSuccess {
    static readonly type = '[API] Delete Server Files Success';

    constructor(public results: Array<IFileOperationResult>) { }
}

export class LoadServerFiles {
    readonly type = '[File Listing] Load Server Files';
}

export class LoadServerFilesFailed {
    static readonly type = '[API] Load Server Files Failed';
}

export class LoadServerFilesSuccess {
    static readonly type = '[API] Load Server Files Success';

    constructor(public results: Array<IFileInfo>) { }
}

export class UploadFiles {
    readonly type = '[Upload] Upload Files';

    constructor(public files: Array<string>) { }
}

export class UploadFilesFailed {
    static readonly type = '[API] Upload Files Failed';
}

export class UploadFilesSuccess {
    static readonly type = '[API] Upload Files Success';

    constructor(public results: Array<IFileOperationResult>) { }
}
