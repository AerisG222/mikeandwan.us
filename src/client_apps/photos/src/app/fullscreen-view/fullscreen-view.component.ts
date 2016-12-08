import { Component, Input } from '@angular/core';

import { PhotoListContext } from '../shared/photo-list-context.model';

@Component({
    selector: 'app-fullscreen-view',
    templateUrl: './fullscreen-view.component.html',
    styleUrls: [ './fullscreen-view.component.css' ]
})
export class FullscreenViewComponent {
    @Input() context: PhotoListContext;
}
