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
import { TurtleScoreComponent } from './turtle-score.component';

describe('Component: TurtleScore', () => {
    let builder: TestComponentBuilder;

    beforeEachProviders(() => [TurtleScoreComponent]);
    beforeEach(inject([TestComponentBuilder], function (tcb: TestComponentBuilder) {
        builder = tcb;
    }));

    it('should inject the component', inject([TurtleScoreComponent],
        (component: TurtleScoreComponent) => {
            expect(component).toBeTruthy();
        }));

    it('should create the component', inject([], () => {
        return builder.createAsync(TurtleScoreComponentTestController)
            .then((fixture: ComponentFixture<any>) => {
                let query = fixture.debugElement.query(By.directive(TurtleScoreComponent));
                expect(query).toBeTruthy();
                expect(query.componentInstance).toBeTruthy();
            });
    }));
});

@Component({
    selector: 'test',
    template: `
    <app-turtle-score></app-turtle-score>
  `,
    directives: [TurtleScoreComponent]
})
class TurtleScoreComponentTestController {
}

