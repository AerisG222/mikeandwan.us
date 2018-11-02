import { Component, Input } from '@angular/core';

import { SvgIcon } from 'maw-common';
import { PhotoListContext } from '../models/photo-list-context.model';

@Component({
    selector: 'app-slideshow-button',
    templateUrl: './slideshow-button.component.html',
    styleUrls: [ './slideshow-button.component.css' ]
})
export class SlideshowButtonComponent {
    @Input() context: PhotoListContext;

    get svgIcon(): SvgIcon {
        if (this.context === null) {
            return SvgIcon.Play;
        }

        return this.context.isSlideshowPlaying ? SvgIcon.Stop : SvgIcon.Play;
    }

    get tooltip(): String {
        if (this.context === null) {
            return '';
        }

        return this.context.isSlideshowPlaying ? 'stop slideshow' : 'start slideshow';
    }
}
