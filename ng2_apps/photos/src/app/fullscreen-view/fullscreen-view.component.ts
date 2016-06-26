import { Component, Input } from '@angular/core';
import { RouterLink } from '@angular/router';

import { PhotoListContext } from '../shared/photo-list-context.model';
import { CategoryLinkComponent } from '../category-link/category-link.component';
import { SlideshowButtonComponent } from '../slideshow-button/slideshow-button.component';

@Component({
    moduleId: module.id,
    selector: 'app-fullscreen-view',
    directives: [ RouterLink, CategoryLinkComponent, SlideshowButtonComponent ],
    templateUrl: 'fullscreen-view.component.html',
    styleUrls: [ 'fullscreen-view.component.css' ]
})
export class FullscreenViewComponent {
    @Input() context: PhotoListContext;
}
