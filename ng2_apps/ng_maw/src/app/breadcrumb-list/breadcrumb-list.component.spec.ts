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
import { BreadcrumbListComponent } from './breadcrumb-list.component';

describe('Component: BreadcrumbList', () => {
    let builder: TestComponentBuilder;

    beforeEachProviders(() => [BreadcrumbListComponent]);
    beforeEach(inject([TestComponentBuilder], function (tcb: TestComponentBuilder) {
        builder = tcb;
    }));

    it('should inject the component', inject([BreadcrumbListComponent],
        (component: BreadcrumbListComponent) => {
            expect(component).toBeTruthy();
        }));

    it('should create the component', inject([], () => {
        return builder.createAsync(BreadcrumbListComponentTestController)
            .then((fixture: ComponentFixture<any>) => {
                let query = fixture.debugElement.query(By.directive(BreadcrumbListComponent));
                expect(query).toBeTruthy();
                expect(query.componentInstance).toBeTruthy();
            });
    }));
});

@Component({
    selector: 'test',
    template: `
    <app-breadcrumb-list></app-breadcrumb-list>
  `,
    directives: [BreadcrumbListComponent]
})
class BreadcrumbListComponentTestController {
}

