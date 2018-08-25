import { Component, OnInit } from '@angular/core';
import { Store, Select } from '@ngxs/store';
import { Observable } from 'rxjs';

import { LoadServerFiles } from '../state/upload.actions';
import { UploadState } from '../state/upload.state';
import { IFileInfo } from '../models/ifile-info';
import { FileSizePipe } from '../pipes/file-size.pipe';
import { RelativeDatePipe } from '../pipes/relative-date.pipe';
import { AuthState } from '../state/auth.state';

@Component({
    selector: 'app-file-listing',
    templateUrl: './file-listing.component.html',
    styleUrls: ['./file-listing.component.css'],
    providers: [
        FileSizePipe,
        RelativeDatePipe
    ]
})
export class FileListingComponent implements OnInit {
    @Select(UploadState.getServerFiles) files$: Observable<Array<IFileInfo>>;
    @Select(AuthState.getShowUsername) showUsername$: Observable<boolean>;

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
