import { Component, Input, HostBinding } from '@angular/core';

import { IPhoto } from '../models/iphoto.model';

@Component({
    selector: 'app-photo-card',
    templateUrl: './photo-card.component.html',
    styleUrls: ['./photo-card.component.css']
})
export class PhotoCardComponent {
    @HostBinding('class.card') cardClass = true;
    @HostBinding('class.active') @Input() active = false;
    @Input() photo: IPhoto;
}
