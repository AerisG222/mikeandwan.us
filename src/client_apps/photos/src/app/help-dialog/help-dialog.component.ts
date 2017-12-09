import { Component } from '@angular/core';

import { SvgIcon } from '../svg-icon/svg-icon.enum';

@Component({
    selector: 'app-help-dialog',
    templateUrl: './help-dialog.component.html',
    styleUrls: [ './help-dialog.component.css' ]
})
export class HelpDialogComponent {
    svgIcon = SvgIcon;
    supportCssFilters = (<any>Modernizr).cssfilters;  // workaround for this missing in modernizr type definition
}
