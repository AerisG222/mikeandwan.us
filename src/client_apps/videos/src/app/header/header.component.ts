import { Component } from '@angular/core';

import { SvgIcon } from '../../ng_maw/svg-icon/svg-icon.enum';

import { VideoStateService } from '../shared/video-state.service';

@Component({
    selector: 'app-header',
    templateUrl: './header.component.html',
    styleUrls: [ './header.component.css' ]
})
export class HeaderComponent {
    svgIcon = SvgIcon;

    constructor(private _stateService: VideoStateService) {

    }

    clickConfig(): void {
        this._stateService.showPreferencesDialog();
    }
}
