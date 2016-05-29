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
import { ChoosePlayerComponent } from './choose-player.component';

describe('Component: ChoosePlayer', () => {
  let builder: TestComponentBuilder;

  beforeEachProviders(() => [ChoosePlayerComponent]);
  beforeEach(inject([TestComponentBuilder], function (tcb: TestComponentBuilder) {
    builder = tcb;
  }));

  it('should inject the component', inject([ChoosePlayerComponent],
      (component: ChoosePlayerComponent) => {
    expect(component).toBeTruthy();
  }));

  it('should create the component', inject([], () => {
    return builder.createAsync(ChoosePlayerComponentTestController)
      .then((fixture: ComponentFixture<any>) => {
        let query = fixture.debugElement.query(By.directive(ChoosePlayerComponent));
        expect(query).toBeTruthy();
        expect(query.componentInstance).toBeTruthy();
      });
  }));
});

@Component({
  selector: 'test',
  template: `
    <app-choose-player></app-choose-player>
  `,
  directives: [ChoosePlayerComponent]
})
class ChoosePlayerComponentTestController {
}

