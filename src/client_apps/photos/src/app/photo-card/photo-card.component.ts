import { Component, Input, HostBinding } from '@angular/core';

import { IPhoto } from '../shared/iphoto.model';

@Component({
    selector: 'app-photo-card',
    templateUrl: './photo-card.component.html',
    styleUrls: ['./photo-card.component.css']
})
export class PhotoCardComponent {
    @HostBinding('class') classes = 'card';
    @Input() photo: IPhoto;
}
