import { Directive, ElementRef, HostListener } from '@angular/core';

// https://gist.github.com/AlejandroPerezMartin/ecd014cb8104c235b582f3a3e1649cf7
@Directive({
    selector: '[mawFocusRemover]'
})
export class MawFocusRemoverDirective {
    constructor(private elRef: ElementRef) {

    }

    @HostListener('click') onClick() {
        this.elRef.nativeElement.blur();
    }
}
