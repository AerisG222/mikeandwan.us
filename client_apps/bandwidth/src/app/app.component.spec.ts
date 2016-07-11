import {
  beforeEachProviders,
  describe,
  expect,
  it,
  inject
} from '@angular/core/testing';
import { BandwidthAppComponent } from '../app/app.component';

beforeEachProviders(() => [BandwidthAppComponent]);

describe('App: Bandwidth', () => {
  it('should create the app',
      inject([BandwidthAppComponent], (app: BandwidthAppComponent) => {
    expect(app).toBeTruthy();
  }));

/*
  it('should have as title \'bandwidth works!\'',
      inject([BandwidthAppComponent], (app: BandwidthAppComponent) => {
    expect(app.title).toEqual('bandwidth works!');
  }));
*/
});
