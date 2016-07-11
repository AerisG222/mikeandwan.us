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
import { CategoryListComponent } from './category-list.component';

describe('Component: CategoryList', () => {
    let builder: TestComponentBuilder;

    beforeEachProviders(() => [CategoryListComponent]);
    beforeEach(inject([TestComponentBuilder], function (tcb: TestComponentBuilder) {
        builder = tcb;
    }));

    it('should inject the component', inject([CategoryListComponent],
        (component: CategoryListComponent) => {
            expect(component).toBeTruthy();
        }));

    it('should create the component', inject([], () => {
        return builder.createAsync(CategoryListComponentTestController)
            .then((fixture: ComponentFixture<any>) => {
                let query = fixture.debugElement.query(By.directive(CategoryListComponent));
                expect(query).toBeTruthy();
                expect(query.componentInstance).toBeTruthy();
            });
    }));
});

@Component({
    selector: 'test',
    template: `
    <app-category-list></app-category-list>
  `,
    directives: [CategoryListComponent]
})
class CategoryListComponentTestController {
}

