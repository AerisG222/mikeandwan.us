import { Component, Input } from '@angular/core';

import { SvgIcon } from 'maw-common';
import { PhotoListContext } from '../models/photo-list-context.model';

@Component({
    selector: 'app-fullscreen-view',
    templateUrl: './fullscreen-view.component.html',
    styleUrls: [ './fullscreen-view.component.css' ]
})
export class FullscreenViewComponent {
    svgIcon = SvgIcon;
    @Input() context: PhotoListContext;
}
