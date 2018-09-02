import { IFileInfo } from './ifile-info';

export interface IFileOperationResult {
    operation: string;
    relativePathSpecified: string;
    UploadedFile: IFileInfo;
    wasSuccessful: boolean;
    error: string;
}
