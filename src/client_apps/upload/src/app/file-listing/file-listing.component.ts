import { Component, OnInit } from '@angular/core';
import { FileViewModel } from './file-view-model';
import { from, Observable } from 'rxjs';
import { Store } from '@ngxs/store';
import * as actions from '../state/actions';

@Component({
    selector: 'app-file-listing',
    templateUrl: './file-listing.component.html',
    styleUrls: ['./file-listing.component.css']
})
export class FileListingComponent implements OnInit {
    showUsername = true;
    files: Array<FileViewModel>;

    constructor(private _store: Store) {

    }

    ngOnInit(): void {
        this._store.dispatch(new actions.LoadServerFiles());

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

    getFiles(): Observable<FileViewModel> {
        return from(this.files)
            .pipe(

            );
    }

    downloadSelected(): void {

    }

    deleteSelected(): void {

    }
}
