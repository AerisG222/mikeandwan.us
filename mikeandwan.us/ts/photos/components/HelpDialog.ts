import { Component, ViewChild } from '@angular/core';
import { NgIf } from '@angular/common';
import { Dialog } from '../../ng_maw/dialog/Dialog';

@Component({
    selector: 'help',	
    directives: [ NgIf, Dialog ],
    templateUrl: '/js/photos/components/HelpDialog.html'
})
export class HelpDialog {
    @ViewChild(Dialog) dialog : Dialog;
	supportCssFilters = (<any>Modernizr).cssfilters;  // workaround for this missing in modernizr type definition
	
    toggle() : void {
        if(this.dialog.isVisible) {
            this.dialog.hide();
        }
        else {
            this.dialog.show();
        }
    }
}
