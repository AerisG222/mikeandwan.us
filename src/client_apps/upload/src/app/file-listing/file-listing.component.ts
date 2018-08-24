import { Component, OnInit } from '@angular/core';
import { Store, Select } from '@ngxs/store';
import { Observable } from 'rxjs';

import { LoadServerFiles } from '../state/upload.actions';
import { UploadState } from '../state/upload.state';
import { IFileInfo } from '../models/ifile-info';
import { FileSizePipe } from '../pipes/file-size.pipe';

@Component({
    selector: 'app-file-listing',
    templateUrl: './file-listing.component.html',
    styleUrls: ['./file-listing.component.css'],
    providers: [ FileSizePipe ]
})
export class FileListingComponent implements OnInit {
    @Select(UploadState.getServerFiles) files$: Observable<Array<IFileInfo>>;
    @Select(UploadState.getShowUsername) showUsername$: Observable<boolean>;

    constructor(private _store: Store) {

    }

    ngOnInit(): void {
        this._store.dispatch(new LoadServerFiles());
    }

    downloadSelected(): void {

    }

    deleteSelected(): void {

    }
}
