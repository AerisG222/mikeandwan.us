import { Component, Input, Output, EventEmitter } from '@angular/core';
import { IVideo } from '../models/ivideo.model';

@Component({
    selector: 'app-video-card-grid',
    templateUrl: './video-card-grid.component.html',
    styleUrls: ['./video-card-grid.component.css']
})
export class VideoCardGridComponent {
    @Input() videoList: Array<IVideo>;
    @Input() cardsPerPage: number;
    @Input() page: number;
    @Input() activeVideo: IVideo;
    @Output() videoSelected = new EventEmitter<IVideo>();

    selectVideo(video: IVideo) {
        this.videoSelected.emit(video);
    }
}
