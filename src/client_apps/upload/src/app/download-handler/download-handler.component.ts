import { Component } from '@angular/core';
import { Select } from '@ngxs/store';
import { UploadState } from '../state/upload.state';
import { Observable } from 'rxjs';

@Component({
    selector: 'app-download-handler',
    templateUrl: './download-handler.component.html',
    styleUrls: ['./download-handler.component.css']
})
export class DownloadHandlerComponent {
    @Select(UploadState.getDownloadError) error$: Observable<any>;

    constructor() { }
}
