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
import { WinnerScreenComponent } from './winner-screen.component';

describe('Component: WinnerScreen', () => {
  let builder: TestComponentBuilder;

  beforeEachProviders(() => [WinnerScreenComponent]);
  beforeEach(inject([TestComponentBuilder], function (tcb: TestComponentBuilder) {
    builder = tcb;
  }));

  it('should inject the component', inject([WinnerScreenComponent],
      (component: WinnerScreenComponent) => {
    expect(component).toBeTruthy();
  }));

  it('should create the component', inject([], () => {
    return builder.createAsync(WinnerScreenComponentTestController)
      .then((fixture: ComponentFixture<any>) => {
        let query = fixture.debugElement.query(By.directive(WinnerScreenComponent));
        expect(query).toBeTruthy();
        expect(query.componentInstance).toBeTruthy();
      });
  }));
});

@Component({
  selector: 'test',
  template: `
    <app-winner-screen></app-winner-screen>
  `,
  directives: [WinnerScreenComponent]
})
class WinnerScreenComponentTestController {
}

