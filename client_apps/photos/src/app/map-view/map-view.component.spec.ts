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
import { MapViewComponent } from './map-view.component';

describe('Component: MapView', () => {
    let builder: TestComponentBuilder;

    beforeEachProviders(() => [MapViewComponent]);
    beforeEach(inject([TestComponentBuilder], function (tcb: TestComponentBuilder) {
        builder = tcb;
    }));

    it('should inject the component', inject([MapViewComponent],
        (component: MapViewComponent) => {
            expect(component).toBeTruthy();
        }));

    it('should create the component', inject([], () => {
        return builder.createAsync(MapViewComponentTestController)
            .then((fixture: ComponentFixture<any>) => {
                let query = fixture.debugElement.query(By.directive(MapViewComponent));
                expect(query).toBeTruthy();
                expect(query.componentInstance).toBeTruthy();
            });
    }));
});

@Component({
    selector: 'test',
    template: `
    <app-map-view></app-map-view>
  `,
    directives: [MapViewComponent]
})
class MapViewComponentTestController {
}

