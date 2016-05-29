import {
  describe,
  ddescribe,
  expect,
  iit,
  it
} from '@angular/core/testing';
import {Config} from './config';

describe('Config', () => {
  it('should create an instance', () => {
    expect(new Config()).toBeTruthy();
  });
});
