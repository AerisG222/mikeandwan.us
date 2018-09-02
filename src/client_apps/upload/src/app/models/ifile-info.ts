import { IFileLocation } from './ifile-location';

export interface IFileInfo {
    location: IFileLocation;
    creationTime: Date;
    sizeInBytes: number;
}
