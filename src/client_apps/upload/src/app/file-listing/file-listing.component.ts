import { Component, OnInit } from '@angular/core';
import { Store, Select } from '@ngxs/store';
import { Observable } from 'rxjs';

import { LoadServerFiles } from '../state/upload.actions';
import { UploadState } from '../state/upload.state';
import { IFileInfo } from '../models/ifile-info';

@Component({
    selector: 'app-file-listing',
    templateUrl: './file-listing.component.html',
    styleUrls: ['./file-listing.component.css']
})
export class FileListingComponent implements OnInit {
    @Select(UploadState.getServerFiles) files$: Observable<Array<IFileInfo>>;
    @Select(UploadState.getShowUsername) showUsername$: Observable<boolean>;

    constructor(private _store: Store) {

    }

    ngOnInit(): void {
        this._store.dispatch(new LoadServerFiles());

        /*
        // TODO: unsubscribe
        this._store.pipe(
            select(''),
            map(x => new FileViewModel(x.username,
                x.filename,
                x.creationTime,
                x.sizeInBytes,
                x.relativePath)
)           )
            .subscribe(
                state => {
                    // this.files = state.files;
                }
            );
        */
    }

    downloadSelected(): void {

    }

    deleteSelected(): void {

    }
}
