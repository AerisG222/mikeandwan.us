import { IFileInfo } from '../models/ifile-info';
import { IFileLocation } from '../models/ifile-location';

export class FileViewModel implements IFileInfo {
    constructor(public location: IFileLocation,
                public creationTime: Date,
                public sizeInBytes: number,
                public isChecked = false) {

    }
}
