/* eslint-disable @typescript-eslint/no-unused-vars */

import { TestBed, waitForAsync } from '@angular/core/testing';
import { CanPlayGuard } from './can-play-guard';

describe('Component: CanPlayGuard', () => {
  it('should create an instance', () => {
    const component = new CanPlayGuard(null, null);
    expect(component).toBeTruthy();
  });
});
