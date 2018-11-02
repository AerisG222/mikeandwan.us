import { Component, OnInit, OnDestroy } from '@angular/core';
import { Store, Select } from '@ngxs/store';
import { Observable, Subject } from 'rxjs';

import { LoadServerFiles, DownloadServerFiles, DeleteServerFiles } from '../state/upload.actions';
import { UploadState } from '../state/upload.state';
import { IFileInfo } from '../models/ifile-info';
import { FileSizePipe } from '../pipes/file-size.pipe';
import { RelativeDatePipe } from '../pipes/relative-date.pipe';
import { AuthState } from '../state/auth.state';
import { FileViewModel } from './file-view-model';
import { map, takeUntil } from 'rxjs/operators';
import { listItemAnimation } from '../animations/animations';
import { SvgIcon } from 'maw-common';

@Component({
    selector: 'app-file-listing',
    templateUrl: './file-listing.component.html',
    styleUrls: ['./file-listing.component.css'],
    providers: [
        FileSizePipe,
        RelativeDatePipe
    ],
    animations: [
        listItemAnimation
    ]
})
export class FileListingComponent implements OnInit, OnDestroy {
    svgIcon = SvgIcon;

    private _unsubscribe: Subject<void> = new Subject();
    files: FileViewModel[] = [];
    @Select(UploadState.getServerFiles) sourceFiles$: Observable<IFileInfo[]>;
    @Select(AuthState.getShowUsername) showUsername$: Observable<boolean>;

    constructor(private _store: Store) {

    }

    ngOnInit(): void {
        this.sourceFiles$
            .pipe(
                takeUntil(this._unsubscribe),
                map(files => this.generateViewModel(files))
            )
            .subscribe(
                files => this.files = files
            );

        this._store.dispatch(new LoadServerFiles());
    }

    ngOnDestroy(): void {
        this._unsubscribe.next();
        this._unsubscribe.complete();
    }

    downloadSingle(file: FileViewModel) {
        this._store.dispatch(new DownloadServerFiles(file.location.relativePath));
    }

    deleteSingle(file: FileViewModel) {
        this._store.dispatch(new DeleteServerFiles(file.location.relativePath));
    }

    downloadSelected(): void {
        const list = this.getSelected();

        this._store.dispatch(new DownloadServerFiles(list));
    }

    deleteSelected(): void {
        const list = this.getSelected();

        this._store.dispatch(new DeleteServerFiles(list));
    }

    getSelected(): string[] {
        return this.files
            .filter(x => x.isChecked)
            .map(x => x.location.relativePath);
    }

    generateViewModel(files: IFileInfo[]): FileViewModel[] {
        const result = [];

        for (const file of files) {
            result.push(new FileViewModel(file.location,
                file.creationTime,
                file.sizeInBytes)
            );
        }

        return result;
    }

    toggleFiles(isChecked): void {
        this.files.forEach(x => {
            x.isChecked = isChecked;
        });
    }

    trackByFile(index, item): number {
        return item.location.relativePath;
    }
}
