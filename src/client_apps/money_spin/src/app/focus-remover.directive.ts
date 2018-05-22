import { Directive, ElementRef, Renderer, HostListener } from '@angular/core';

// https://gist.github.com/AlejandroPerezMartin/ecd014cb8104c235b582f3a3e1649cf7
@Directive({
    selector: '[appFocusRemover]'
})
export class FocusRemoverDirective {
    constructor(private elRef: ElementRef,
        private renderer: Renderer) {

    }

    @HostListener('click') onClick() {
        this.renderer.invokeElementMethod(this.elRef.nativeElement, 'blur', []);
    }
}
