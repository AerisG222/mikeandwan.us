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
import { YearListComponent } from './year-list.component';

describe('Component: YearList', () => {
    let builder: TestComponentBuilder;

    beforeEachProviders(() => [YearListComponent]);
    beforeEach(inject([TestComponentBuilder], function (tcb: TestComponentBuilder) {
        builder = tcb;
    }));

    it('should inject the component', inject([YearListComponent],
        (component: YearListComponent) => {
            expect(component).toBeTruthy();
        }));

    it('should create the component', inject([], () => {
        return builder.createAsync(YearListComponentTestController)
            .then((fixture: ComponentFixture<any>) => {
                let query = fixture.debugElement.query(By.directive(YearListComponent));
                expect(query).toBeTruthy();
                expect(query.componentInstance).toBeTruthy();
            });
    }));
});

@Component({
    selector: 'test',
    template: `
    <app-year-list></app-year-list>
  `,
    directives: [YearListComponent]
})
class YearListComponentTestController {
}

