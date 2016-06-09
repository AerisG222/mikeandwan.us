import {
    beforeEach,
    beforeEachProviders,
    describe,
    expect,
    it,
    inject,
} from '@angular/core/testing';
import { ComponentFixture, TestComponentBuilder } from '@angular/compiler/testing';
import { Component } from '@angular/core';
import { By } from '@angular/platform-browser';
import { FullscreenViewComponent } from './fullscreen-view.component';

describe('Component: FullscreenView', () => {
    let builder: TestComponentBuilder;

    beforeEachProviders(() => [FullscreenViewComponent]);
    beforeEach(inject([TestComponentBuilder], function (tcb: TestComponentBuilder) {
        builder = tcb;
    }));

    it('should inject the component', inject([FullscreenViewComponent],
        (component: FullscreenViewComponent) => {
            expect(component).toBeTruthy();
        }));

    it('should create the component', inject([], () => {
        return builder.createAsync(FullscreenViewComponentTestController)
            .then((fixture: ComponentFixture<any>) => {
                let query = fixture.debugElement.query(By.directive(FullscreenViewComponent));
                expect(query).toBeTruthy();
                expect(query.componentInstance).toBeTruthy();
            });
    }));
});

@Component({
    selector: 'test',
    template: `
    <app-fullscreen-view></app-fullscreen-view>
  `,
    directives: [FullscreenViewComponent]
})
class FullscreenViewComponentTestController {
}

