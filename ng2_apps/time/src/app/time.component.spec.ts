import {
  beforeEachProviders,
  describe,
  expect,
  it,
  inject
} from '@angular/core/testing';
import { TimeAppComponent } from '../app/time.component';

beforeEachProviders(() => [TimeAppComponent]);

describe('App: Time', () => {
  it('should create the app',
      inject([TimeAppComponent], (app: TimeAppComponent) => {
    expect(app).toBeTruthy();
  }));

/*
  it('should have as title \'time works!\'',
      inject([TimeAppComponent], (app: TimeAppComponent) => {
    expect(app.title).toEqual('time works!');
  }));
*/
});
