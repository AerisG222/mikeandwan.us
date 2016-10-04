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
import { SplashScreenComponent } from './splash-screen.component';

describe('Component: SplashScreen', () => {
    let builder: TestComponentBuilder;

    beforeEachProviders(() => [SplashScreenComponent]);
    beforeEach(inject([TestComponentBuilder], function (tcb: TestComponentBuilder) {
        builder = tcb;
    }));

    it('should inject the component', inject([SplashScreenComponent],
        (component: SplashScreenComponent) => {
            expect(component).toBeTruthy();
        }));

    it('should create the component', inject([], () => {
        return builder.createAsync(SplashScreenComponentTestController)
            .then((fixture: ComponentFixture<any>) => {
                let query = fixture.debugElement.query(By.directive(SplashScreenComponent));
                expect(query).toBeTruthy();
                expect(query.componentInstance).toBeTruthy();
            });
    }));
});

@Component({
    selector: 'test',
    template: `
    <app-splash-screen></app-splash-screen>
  `,
    directives: [SplashScreenComponent]
})
class SplashScreenComponentTestController {
}

