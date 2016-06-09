import {
    beforeEachProviders,
    describe,
    expect,
    it,
    inject
} from '@angular/core/testing';
import { NgMawAppComponent } from '../app/ng-maw.component';

beforeEachProviders(() => [NgMawAppComponent]);

describe('App: NgMaw', () => {
    it('should create the app',
        inject([NgMawAppComponent], (app: NgMawAppComponent) => {
            expect(app).toBeTruthy();
        }));

    it('should have as title \'ng-maw works!\'',
        inject([NgMawAppComponent], (app: NgMawAppComponent) => {
            expect(app.title).toEqual('ng-maw works!');
        }));
});
