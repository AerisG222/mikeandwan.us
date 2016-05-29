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
import { ThumbnailListComponent } from './thumbnail-list.component';

describe('Component: ThumbnailList', () => {
  let builder: TestComponentBuilder;

  beforeEachProviders(() => [ThumbnailListComponent]);
  beforeEach(inject([TestComponentBuilder], function (tcb: TestComponentBuilder) {
    builder = tcb;
  }));

  it('should inject the component', inject([ThumbnailListComponent],
      (component: ThumbnailListComponent) => {
    expect(component).toBeTruthy();
  }));

  it('should create the component', inject([], () => {
    return builder.createAsync(ThumbnailListComponentTestController)
      .then((fixture: ComponentFixture<any>) => {
        let query = fixture.debugElement.query(By.directive(ThumbnailListComponent));
        expect(query).toBeTruthy();
        expect(query.componentInstance).toBeTruthy();
      });
  }));
});

@Component({
  selector: 'test',
  template: `
    <app-thumbnail-list></app-thumbnail-list>
  `,
  directives: [ThumbnailListComponent]
})
class ThumbnailListComponentTestController {
}

