import { Component, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { Observable } from 'rxjs';
import { UploadState } from '../state/upload.state';
import { Select, Store } from '@ngxs/store';
import { InitializeUploader } from '../state/upload.actions';
import { FileSizePipe } from '../pipes/file-size.pipe';
import { SvgIcon } from '../svg-icon/svg-icon.enum';
import { trigger, transition, animate, style } from '@angular/animations';

@Component({
    selector: 'app-upload',
    templateUrl: './upload.component.html',
    styleUrls: ['./upload.component.css'],
    providers: [
        FileSizePipe
    ],
    animations: [
        trigger('itemAnim', [
            transition(':leave', [
                animate('0.5s 0.2s ease', style({
                    opacity: 0
                }))
            ])
        ])
    ]
})
export class UploadComponent implements OnInit {
    svgIcon = SvgIcon;
    hasBaseDropZoneOver = false;

    @Select(UploadState.getUploader) uploader$: Observable<FileUploader>;

    constructor(private _store: Store) {

    }

    ngOnInit() {
        this._store.dispatch(new InitializeUploader());
    }

    fileOverBase(e: any): void {
        this.hasBaseDropZoneOver = e;
    }

    trackByFile(index, item) {
        return item.file.name;
    }
}
