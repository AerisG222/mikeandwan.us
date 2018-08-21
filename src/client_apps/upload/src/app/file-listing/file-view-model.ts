import { IFileInfo } from '../shared/ifile-info';

export class FileViewModel implements IFileInfo {
    isDeleteChecked = false;
    isDownloadChecked = false;

    constructor(public username: string,
                public filename: string,
                public creationTime: Date,
                public sizeInBytes: number,
                public relativePath: string) {

    }
}
