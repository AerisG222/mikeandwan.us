import { Component, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { Observable } from 'rxjs';
import { UploadState } from '../state/upload.state';
import { Select, Store } from '@ngxs/store';
import { InitializeUploader } from '../state/upload.actions';

@Component({
    selector: 'app-upload',
    templateUrl: './upload.component.html',
    styleUrls: ['./upload.component.css']
})
export class UploadComponent implements OnInit {
    @Select(UploadState.getUploader) uploader$: Observable<FileUploader>;

    constructor(private _store: Store) {

    }

    ngOnInit() {
        this._store.dispatch(new InitializeUploader());
    }
}
