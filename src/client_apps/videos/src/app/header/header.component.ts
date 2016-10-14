import { Component } from '@angular/core';

import { VideoStateService } from '../shared/video-state.service';

@Component({
    selector: 'app-header',
    templateUrl: 'header.component.html',
    styleUrls: [ 'header.component.css' ]
})
export class HeaderComponent {
    constructor(private _stateService: VideoStateService) {

    }

    clickConfig(): void {
        this._stateService.showPreferencesDialog();
    };
}
