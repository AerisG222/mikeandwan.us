import { Component, Input, Output, EventEmitter, ElementRef, AfterViewInit } from '@angular/core';
import { NgFor, NgIf, NgClass } from '@angular/common';

import { DialogButton } from './dialog-button.model';
declare var jQuery: any;

@Component({
    moduleId: module.id,
    selector: 'maw-dialog',
    directives: [ NgIf, NgFor, NgClass ],
    templateUrl: 'dialog.component.html',
    styleUrls: [ 'dialog.component.css' ]
})
export class DialogComponent implements AfterViewInit {
    private _modalDiv: HTMLDivElement = null;
    isVisible: boolean = false;
    @Input() title: string = null;
    buttons: Array<DialogButton> = [];
    @Output() buttonClick: EventEmitter<string> = new EventEmitter<string>();
    @Output() cancel: EventEmitter<any> = new EventEmitter<any>();
    @Input() sizeClass: string;

    constructor(private _elRef: ElementRef) {

    }

    ngAfterViewInit(): void {
        this._modalDiv = <HTMLDivElement>(<HTMLElement>this._elRef.nativeElement).querySelector('div.modal');

        jQuery(this._modalDiv).modal({ show: false });
        jQuery(this._modalDiv).on('hidden.bs.modal', (e: any) => this.onCancel());
    }

    show(): void {
        this.setVisibility(true);
    }

    hide(): void {
        this.setVisibility(false);
    }

    setVisibility(isShown: boolean): void {
        let doShow = isShown ? 'show' : 'hide';

        jQuery(this._modalDiv).modal(doShow);

        this.isVisible = isShown;
    }

    onCancel(): void {
        this.hide();
        this.cancel.next(null);
    }

    onButtonClick(btn: DialogButton): void {
        this.buttonClick.next(btn);
    }
}
