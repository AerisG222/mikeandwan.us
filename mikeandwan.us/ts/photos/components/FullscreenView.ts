import { Component, Input } from '@angular/core';
import { RouterLink } from '@angular/router-deprecated';
import { PhotoListContext } from '../models/PhotoListContext';
import { CategoryLink } from './CategoryLink';
import { SlideshowButton } from './SlideshowButton';

@Component({
    selector: 'fullscreen',
    directives: [ RouterLink, CategoryLink, SlideshowButton ],
    templateUrl: '/js/photos/components/FullscreenView.html'
})
export class FullscreenView {
	@Input() context : PhotoListContext;
}
