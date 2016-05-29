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
import { SlideshowButtonComponent } from './slideshow-button.component';

describe('Component: SlideshowButton', () => {
  let builder: TestComponentBuilder;

  beforeEachProviders(() => [SlideshowButtonComponent]);
  beforeEach(inject([TestComponentBuilder], function (tcb: TestComponentBuilder) {
    builder = tcb;
  }));

  it('should inject the component', inject([SlideshowButtonComponent],
      (component: SlideshowButtonComponent) => {
    expect(component).toBeTruthy();
  }));

  it('should create the component', inject([], () => {
    return builder.createAsync(SlideshowButtonComponentTestController)
      .then((fixture: ComponentFixture<any>) => {
        let query = fixture.debugElement.query(By.directive(SlideshowButtonComponent));
        expect(query).toBeTruthy();
        expect(query.componentInstance).toBeTruthy();
      });
  }));
});

@Component({
  selector: 'test',
  template: `
    <app-slideshow-button></app-slideshow-button>
  `,
  directives: [SlideshowButtonComponent]
})
class SlideshowButtonComponentTestController {
}

