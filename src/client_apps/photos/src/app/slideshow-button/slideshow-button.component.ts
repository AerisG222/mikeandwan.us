import { Component, Input } from '@angular/core';

import { SvgIcon } from '../../ng_maw/svg-icon/svg-icon.enum';

import { PhotoListContext } from '../shared/photo-list-context.model';

@Component({
    selector: 'app-slideshow-button',
    templateUrl: './slideshow-button.component.html',
    styleUrls: [ './slideshow-button.component.css' ]
})
export class SlideshowButtonComponent {
    svgIcon = SvgIcon;
    @Input() context: PhotoListContext;
}
