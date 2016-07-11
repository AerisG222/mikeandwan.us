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
import { CommentViewComponent } from './comment-view.component';

describe('Component: CommentView', () => {
    let builder: TestComponentBuilder;

    beforeEachProviders(() => [CommentViewComponent]);
    beforeEach(inject([TestComponentBuilder], function (tcb: TestComponentBuilder) {
        builder = tcb;
    }));

    it('should inject the component', inject([CommentViewComponent],
        (component: CommentViewComponent) => {
            expect(component).toBeTruthy();
        }));

    it('should create the component', inject([], () => {
        return builder.createAsync(CommentViewComponentTestController)
            .then((fixture: ComponentFixture<any>) => {
                let query = fixture.debugElement.query(By.directive(CommentViewComponent));
                expect(query).toBeTruthy();
                expect(query.componentInstance).toBeTruthy();
            });
    }));
});

@Component({
    selector: 'test',
    template: `
    <app-comment-view></app-comment-view>
  `,
    directives: [CommentViewComponent]
})
class CommentViewComponentTestController {
}

