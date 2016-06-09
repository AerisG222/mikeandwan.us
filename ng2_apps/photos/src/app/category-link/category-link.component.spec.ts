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
import { CategoryLinkComponent } from './category-link.component';

describe('Component: CategoryLink', () => {
    let builder: TestComponentBuilder;

    beforeEachProviders(() => [CategoryLinkComponent]);
    beforeEach(inject([TestComponentBuilder], function (tcb: TestComponentBuilder) {
        builder = tcb;
    }));

    it('should inject the component', inject([CategoryLinkComponent],
        (component: CategoryLinkComponent) => {
            expect(component).toBeTruthy();
        }));

    it('should create the component', inject([], () => {
        return builder.createAsync(CategoryLinkComponentTestController)
            .then((fixture: ComponentFixture<any>) => {
                let query = fixture.debugElement.query(By.directive(CategoryLinkComponent));
                expect(query).toBeTruthy();
                expect(query.componentInstance).toBeTruthy();
            });
    }));
});

@Component({
    selector: 'test',
    template: `
    <app-category-link></app-category-link>
  `,
    directives: [CategoryLinkComponent]
})
class CategoryLinkComponentTestController {
}

