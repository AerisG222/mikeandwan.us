import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MawFocusRemoverDirective } from './directives/maw-focus-remover.directive';
import { SvgIconComponent } from './svg-icon/svg-icon.component';

@NgModule({
    imports: [
        CommonModule
    ],
    declarations: [
        MawFocusRemoverDirective,
        SvgIconComponent
    ],
    exports: [
        MawFocusRemoverDirective,
        SvgIconComponent
    ]
})
export class MawCommonModule {

}
