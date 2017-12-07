import { Component, Input } from '@angular/core';

import { IPhoto } from '../shared/iphoto.model';

@Component({
    selector: 'app-photo-card',
    templateUrl: './photo-card.component.html',
    styleUrls: ['./photo-card.component.css']
})
export class PhotoCardComponent {
    @Input() photo: IPhoto;
}
