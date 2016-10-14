import { Component, Input } from '@angular/core';

import { PhotoListContext } from '../shared/photo-list-context.model';

@Component({
    selector: 'app-slideshow-button',
    templateUrl: 'slideshow-button.component.html',
    styleUrls: [ 'slideshow-button.component.css' ]
})
export class SlideshowButtonComponent {
    @Input() context: PhotoListContext;
}
