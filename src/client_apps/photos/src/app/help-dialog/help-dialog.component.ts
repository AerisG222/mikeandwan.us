import { Component, ViewChild } from '@angular/core';

import { DialogComponent } from '../../ng_maw/dialog/dialog.component';
import { SvgIcon } from '../../ng_maw/svg-icon/svg-icon.enum';

@Component({
    selector: 'app-help-dialog',
    templateUrl: './help-dialog.component.html',
    styleUrls: [ './help-dialog.component.css' ]
})
export class HelpDialogComponent {
    @ViewChild(DialogComponent) dialog: DialogComponent;
    svgIcon = SvgIcon;
    supportCssFilters = (<any>Modernizr).cssfilters;  // workaround for this missing in modernizr type definition

    toggle(): void {
        if (this.dialog.isVisible) {
            this.dialog.hide();
        } else {
            this.dialog.show();
        }
    }
}
