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
import { PreferenceDialogComponent } from './preference-dialog.component';

describe('Component: PreferenceDialog', () => {
  let builder: TestComponentBuilder;

  beforeEachProviders(() => [PreferenceDialogComponent]);
  beforeEach(inject([TestComponentBuilder], function (tcb: TestComponentBuilder) {
    builder = tcb;
  }));

  it('should inject the component', inject([PreferenceDialogComponent],
      (component: PreferenceDialogComponent) => {
    expect(component).toBeTruthy();
  }));

  it('should create the component', inject([], () => {
    return builder.createAsync(PreferenceDialogComponentTestController)
      .then((fixture: ComponentFixture<any>) => {
        let query = fixture.debugElement.query(By.directive(PreferenceDialogComponent));
        expect(query).toBeTruthy();
        expect(query.componentInstance).toBeTruthy();
      });
  }));
});

@Component({
  selector: 'test',
  template: `
    <app-preference-dialog></app-preference-dialog>
  `,
  directives: [PreferenceDialogComponent]
})
class PreferenceDialogComponentTestController {
}

