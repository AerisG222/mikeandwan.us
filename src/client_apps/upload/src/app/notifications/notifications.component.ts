import { Component, Input } from '@angular/core';
import { IFileOperationResult } from '../models/ifile-operation-result';
import { from, Observable } from 'rxjs';
import { filter } from 'rxjs/operators';


@Component({
    selector: 'app-notifications',
    templateUrl: './notifications.component.html',
    styleUrls: ['./notifications.component.css']
})
export class NotificationsComponent {
   // @Input() results: Array<IFileOperationResult> = [];

    constructor() {

    }
/*
    getSuccessfulResults(): Observable<IFileOperationResult> {
        return from(this.results)
            .pipe(
                filter(x => x.wasSuccessful)
            );
    }

    getUnsuccessfulResults(): Observable<IFileOperationResult> {
        return from(this.results)
            .pipe(
                filter(x => !x.wasSuccessful)
            );
    }
    */
}
