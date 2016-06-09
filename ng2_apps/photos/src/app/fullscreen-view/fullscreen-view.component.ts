import { Component, Input } from '@angular/core';
import { RouterLink } from '@angular/router-deprecated';

import { PhotoListContext } from '../shared';
import { CategoryLinkComponent } from '../category-link';
import { SlideshowButtonComponent } from '../slideshow-button';

@Component({
    moduleId: module.id,
    selector: 'app-fullscreen-view',
    directives: [RouterLink, CategoryLinkComponent, SlideshowButtonComponent],
    templateUrl: 'fullscreen-view.component.html',
    styleUrls: ['fullscreen-view.component.css']
})
export class FullscreenViewComponent {
    @Input() context: PhotoListContext;
}
