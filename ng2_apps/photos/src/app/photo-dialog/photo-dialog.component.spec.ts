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
import { PhotoDialogComponent } from './photo-dialog.component';

describe('Component: PhotoDialog', () => {
    let builder: TestComponentBuilder;

    beforeEachProviders(() => [PhotoDialogComponent]);
    beforeEach(inject([TestComponentBuilder], function (tcb: TestComponentBuilder) {
        builder = tcb;
    }));

    it('should inject the component', inject([PhotoDialogComponent],
        (component: PhotoDialogComponent) => {
            expect(component).toBeTruthy();
        }));

    it('should create the component', inject([], () => {
        return builder.createAsync(PhotoDialogComponentTestController)
            .then((fixture: ComponentFixture<any>) => {
                let query = fixture.debugElement.query(By.directive(PhotoDialogComponent));
                expect(query).toBeTruthy();
                expect(query.componentInstance).toBeTruthy();
            });
    }));
});

@Component({
    selector: 'test',
    template: `
    <app-photo-dialog></app-photo-dialog>
  `,
    directives: [PhotoDialogComponent]
})
class PhotoDialogComponentTestController {
}

