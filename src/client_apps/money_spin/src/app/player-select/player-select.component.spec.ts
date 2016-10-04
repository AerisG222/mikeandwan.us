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
import { PlayerSelectComponent } from './player-select.component';

describe('Component: PlayerSelect', () => {
    let builder: TestComponentBuilder;

    beforeEachProviders(() => [PlayerSelectComponent]);
    beforeEach(inject([TestComponentBuilder], function (tcb: TestComponentBuilder) {
        builder = tcb;
    }));

    it('should inject the component', inject([PlayerSelectComponent],
        (component: PlayerSelectComponent) => {
            expect(component).toBeTruthy();
        }));

    it('should create the component', inject([], () => {
        return builder.createAsync(PlayerSelectComponentTestController)
            .then((fixture: ComponentFixture<any>) => {
                let query = fixture.debugElement.query(By.directive(PlayerSelectComponent));
                expect(query).toBeTruthy();
                expect(query.componentInstance).toBeTruthy();
            });
    }));
});

@Component({
    selector: 'test',
    template: `
    <app-player-select></app-player-select>
  `,
    directives: [PlayerSelectComponent]
})
class PlayerSelectComponentTestController {
}

