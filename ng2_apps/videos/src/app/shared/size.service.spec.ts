import {
  beforeEachProviders,
  it,
  describe,
  expect,
  inject
} from '@angular/core/testing';
import { SizeService } from './size.service';

describe('Size Service', () => {
  beforeEachProviders(() => [SizeService]);

  it('should ...',
      inject([SizeService], (service: SizeService) => {
    expect(service).toBeTruthy();
  }));
});
