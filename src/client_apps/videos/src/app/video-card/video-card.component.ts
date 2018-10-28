import { Component, Input, HostBinding } from '@angular/core';

import { IVideo } from '../models/ivideo.model';

@Component({
    selector: 'app-video-card',
    templateUrl: './video-card.component.html',
    styleUrls: ['./video-card.component.css']
})
export class VideoCardComponent {
    @HostBinding('class.card') cardClass = true;
    @HostBinding('class.active') @Input() active = false;
    @Input() video: IVideo;
}
