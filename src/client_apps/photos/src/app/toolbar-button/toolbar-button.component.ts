import { Component, EventEmitter, Input, Output } from '@angular/core';

import { SvgIcon } from 'maw-common';

@Component({
    selector: 'app-toolbar-button',
    templateUrl: './toolbar-button.component.html',
    styleUrls: ['./toolbar-button.component.css']
})
export class ToolbarButtonComponent {
    @Input() tooltip: string;
    @Input() isActive = false;
    @Input() icon: SvgIcon;
    @Input() iconSize = 'icon-lg';
}
