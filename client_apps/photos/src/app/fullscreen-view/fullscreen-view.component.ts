import { Component, Input } from '@angular/core';

import { PhotoListContext } from '../shared/photo-list-context.model';
import { CategoryLinkComponent } from '../category-link/category-link.component';
import { SlideshowButtonComponent } from '../slideshow-button/slideshow-button.component';

@Component({
    moduleId: module.id,
    selector: 'fullscreen-view',
    directives: [ CategoryLinkComponent, SlideshowButtonComponent ],
    templateUrl: 'fullscreen-view.component.html',
    styleUrls: [ 'fullscreen-view.component.css' ]
})
export class FullscreenViewComponent {
    @Input() context: PhotoListContext;
}
