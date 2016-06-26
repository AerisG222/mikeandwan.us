import { Component, ViewChild } from '@angular/core';
import { NgIf } from '@angular/common';

import { DialogComponent } from '../../../../ng_maw/src/app/dialog/dialog.component';

@Component({
    moduleId: module.id,
    selector: 'app-help-dialog',
    directives: [ NgIf, DialogComponent ],
    templateUrl: 'help-dialog.component.html',
    styleUrls: [ 'help-dialog.component.css' ]
})
export class HelpDialogComponent {
    @ViewChild(DialogComponent) dialog: DialogComponent;
    supportCssFilters = (<any>Modernizr).cssfilters;  // workaround for this missing in modernizr type definition

    toggle(): void {
        if (this.dialog.isVisible) {
            this.dialog.hide();
        }
        else {
            this.dialog.show();
        }
    }
}
