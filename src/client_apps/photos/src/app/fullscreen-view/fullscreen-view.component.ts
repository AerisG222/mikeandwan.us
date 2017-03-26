import { Component, Input } from '@angular/core';

import { SvgIcon } from '../../ng_maw/svg-icon/svg-icon.enum';

import { PhotoListContext } from '../shared/photo-list-context.model';

@Component({
    selector: 'app-fullscreen-view',
    templateUrl: './fullscreen-view.component.html',
    styleUrls: [ './fullscreen-view.component.css' ]
})
export class FullscreenViewComponent {
    svgIcon = SvgIcon;
    @Input() context: PhotoListContext;
}
