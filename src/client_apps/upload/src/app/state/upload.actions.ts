import { IFileInfo } from '../models/ifile-info';
import { IFileOperationResult } from '../models/ifile-operation-result';

export class DeleteServerFiles {
    static readonly type = '[File Listing] Delete Server Files';

    constructor(public files: string | string[]) { }
}

export class DeleteServerFilesFailed {
    static readonly type = '[API] Delete Server Files Failed';

    constructor(public error: any) { }
}

export class DeleteServerFilesSuccess {
    static readonly type = '[API] Delete Server Files Success';

    constructor(public results: IFileOperationResult[]) { }
}

export class DownloadServerFiles {
    static readonly type = '[File Listing] Download Server Files';

    constructor(public files: string | string[]) { }
}

export class DownloadServerFilesSuccess {
    static readonly type = '[API] Download Server Files Success';

    constructor(public file: any) { }
}

export class DownloadServerFilesError {
    static readonly type = '[API] Download Server Files Error';

    constructor(err: string) { }
}

export class FileAdded {
    static readonly  type = '[SignalR] File Added';

    constructor(public file: IFileInfo) { }
}

export class FileDeleted {
    static readonly  type = '[SignalR] File Deleted';

    constructor(public file: IFileInfo) { }
}

export class InitializeUploader {
    static readonly type = '[Upload Component] Initialize Uploader';

    constructor() { }
}

export class LoadServerFiles {
    static readonly type = '[File Listing] Load Server Files';
}

export class LoadServerFilesFailed {
    static readonly type = '[API] Load Server Files Failed';

    constructor(public error: any) { }
}

export class LoadServerFilesSuccess {
    static readonly type = '[API] Load Server Files Success';

    constructor(public results: IFileInfo[]) { }
}

export class UploadFiles {
    static readonly type = '[Upload] Upload Files';

    constructor(public files: string[]) { }
}

export class UploadFilesFailed {
    static readonly type = '[API] Upload Files Failed';

    constructor(public error: any) { }
}

export class UploadFilesSuccess {
    static readonly type = '[API] Upload Files Success';

    constructor(public results: IFileOperationResult[]) { }
}
