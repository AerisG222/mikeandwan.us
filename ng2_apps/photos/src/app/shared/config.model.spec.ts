import {
  describe,
  ddescribe,
  expect,
  iit,
  it
} from '@angular/core/testing';
import {Config} from './config.model';

describe('Config', () => {
  it('should create an instance', () => {
    expect(new Config()).toBeTruthy();
  });
});
