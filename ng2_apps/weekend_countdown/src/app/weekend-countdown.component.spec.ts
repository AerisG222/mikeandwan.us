import {
  beforeEachProviders,
  describe,
  expect,
  it,
  inject
} from '@angular/core/testing';
import { WeekendCountdownAppComponent } from '../app/weekend-countdown.component';

beforeEachProviders(() => [WeekendCountdownAppComponent]);

describe('App: WeekendCountdown', () => {
  it('should create the app',
      inject([WeekendCountdownAppComponent], (app: WeekendCountdownAppComponent) => {
    expect(app).toBeTruthy();
  }));

/*
  it('should have as title \'weekend-countdown works!\'',
      inject([WeekendCountdownAppComponent], (app: WeekendCountdownAppComponent) => {
    expect(app.title).toEqual('weekend-countdown works!');
  }));
*/
});
