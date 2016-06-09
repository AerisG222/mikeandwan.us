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
import { ChooseTurtleGridComponent } from './choose-turtle-grid.component';

describe('Component: ChooseTurtleGrid', () => {
    let builder: TestComponentBuilder;

    beforeEachProviders(() => [ChooseTurtleGridComponent]);
    beforeEach(inject([TestComponentBuilder], function (tcb: TestComponentBuilder) {
        builder = tcb;
    }));

    it('should inject the component', inject([ChooseTurtleGridComponent],
        (component: ChooseTurtleGridComponent) => {
            expect(component).toBeTruthy();
        }));

    it('should create the component', inject([], () => {
        return builder.createAsync(ChooseTurtleGridComponentTestController)
            .then((fixture: ComponentFixture<any>) => {
                let query = fixture.debugElement.query(By.directive(ChooseTurtleGridComponent));
                expect(query).toBeTruthy();
                expect(query.componentInstance).toBeTruthy();
            });
    }));
});

@Component({
    selector: 'test',
    template: `
    <app-choose-turtle-grid></app-choose-turtle-grid>
  `,
    directives: [ChooseTurtleGridComponent]
})
class ChooseTurtleGridComponentTestController {
}

