import { Component, Input } from '@angular/core';
import { PhotoListContext } from '../models/PhotoListContext';

@Component({
    selector: 'slideshowbutton',
    templateUrl: '/js/photos/components/SlideshowButton.html'
})
export class SlideshowButton {
    @Input() context : PhotoListContext;
}
