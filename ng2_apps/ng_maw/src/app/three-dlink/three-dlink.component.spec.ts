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
import { ThreeDLinkComponent } from './three-dlink.component';

describe('Component: ThreeDLink', () => {
  let builder: TestComponentBuilder;

  beforeEachProviders(() => [ThreeDLinkComponent]);
  beforeEach(inject([TestComponentBuilder], function (tcb: TestComponentBuilder) {
    builder = tcb;
  }));

  it('should inject the component', inject([ThreeDLinkComponent],
      (component: ThreeDLinkComponent) => {
    expect(component).toBeTruthy();
  }));

  it('should create the component', inject([], () => {
    return builder.createAsync(ThreeDLinkComponentTestController)
      .then((fixture: ComponentFixture<any>) => {
        let query = fixture.debugElement.query(By.directive(ThreeDLinkComponent));
        expect(query).toBeTruthy();
        expect(query.componentInstance).toBeTruthy();
      });
  }));
});

@Component({
  selector: 'test',
  template: `
    <app-three-dlink></app-three-dlink>
  `,
  directives: [ThreeDLinkComponent]
})
class ThreeDLinkComponentTestController {
}

