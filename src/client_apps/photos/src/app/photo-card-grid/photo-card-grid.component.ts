import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Photo } from '../shared/photo.model';

@Component({
    selector: 'app-photo-card-grid',
    templateUrl: './photo-card-grid.component.html',
    styleUrls: ['./photo-card-grid.component.css']
})
export class PhotoCardGridComponent {
    @Input() photoList: Array<Photo>;
    @Input() cardsPerPage: number;
    @Input() page: number;
    @Input() activePhoto: Photo;
    @Output() photoSelected = new EventEmitter<Photo>();

    selectPhoto(photo: Photo) {
        this.photoSelected.emit(photo);
    }
}
