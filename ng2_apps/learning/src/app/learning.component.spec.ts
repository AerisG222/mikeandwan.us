import {
  beforeEachProviders,
  describe,
  expect,
  it,
  inject
} from '@angular/core/testing';
import { LearningAppComponent } from '../app/learning.component';

beforeEachProviders(() => [LearningAppComponent]);

describe('App: Learning', () => {
  it('should create the app',
      inject([LearningAppComponent], (app: LearningAppComponent) => {
    expect(app).toBeTruthy();
  }));

/*
  it('should have as title \'learning works!\'',
      inject([LearningAppComponent], (app: LearningAppComponent) => {
    expect(app.title).toEqual('learning works!');
  }));
*/
});
