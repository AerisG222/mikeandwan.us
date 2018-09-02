import { IFileInfo } from '../models/ifile-info';
import { IFileLocation } from '../models/ifile-location';

export class FileViewModel implements IFileInfo {
    isDeleteChecked = false;
    isDownloadChecked = false;

    constructor(public location: IFileLocation,
                public creationTime: Date,
                public sizeInBytes: number,
                public relativePath: string) {

    }
}
