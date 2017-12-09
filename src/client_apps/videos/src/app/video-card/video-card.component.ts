import { Component, Input, HostBinding } from '@angular/core';

import { IVideo } from '../shared/ivideo.model';

@Component({
    selector: 'app-video-card',
    templateUrl: './video-card.component.html',
    styleUrls: ['./video-card.component.css']
})
export class VideoCardComponent {
    @HostBinding('class') classes = 'card';
    @Input() video: IVideo;
}
