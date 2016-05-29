import {
  beforeEachProviders,
  describe,
  expect,
  it,
  inject
} from '@angular/core/testing';
import { BinaryClockAppComponent } from '../app/binary-clock.component';

beforeEachProviders(() => [BinaryClockAppComponent]);

describe('App: BinaryClock', () => {
  it('should create the app',
      inject([BinaryClockAppComponent], (app: BinaryClockAppComponent) => {
    expect(app).toBeTruthy();
  }));
  
/*
  it('should have as title \'binary-clock works!\'',
      inject([BinaryClockAppComponent], (app: BinaryClockAppComponent) => {
    expect(app.title).toEqual('binary-clock works!');
  }));
*/
});
